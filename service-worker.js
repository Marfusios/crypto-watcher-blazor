/* Manifest version: +NuSd4eK */
// Caution! Be sure you understand the caveats before publishing an application with
// offline support. See https://aka.ms/blazor-offline-considerations

self.importScripts('./service-worker-assets.js');
self.addEventListener('install', event => event.waitUntil(onInstall(event)));
self.addEventListener('activate', event => event.waitUntil(onActivate(event)));
self.addEventListener('fetch', event => {
    // Exchange REST calls (cross-origin) and non GET requests go straight to the network
    if (event.request.method === 'GET' && new URL(event.request.url).origin === self.location.origin) {
        event.respondWith(onFetch(event));
    }
});

const cacheNamePrefix = 'offline-cache-';
const cacheName = `${cacheNamePrefix}${self.assetsManifest.version}`;
const offlineAssetsInclude = [ /\.dll$/, /\.pdb$/, /\.wasm/, /\.html/, /\.js$/, /\.json$/, /\.css$/, /\.woff$/, /\.png$/, /\.jpe?g$/, /\.gif$/, /\.ico$/, /\.blat$/, /\.dat$/ ];
const offlineAssetsExclude = [ /^service-worker\.js$/ ];

// Framework files carry a content hash in their names and never change,
// everything else (index.html, css, manifest) can change with every deploy
const immutableAssets = /\/_framework\//;

async function onInstall(event) {
    console.info('Service worker: Install');

    // Fetch and cache all matching items from the assets manifest
    const assetsRequests = self.assetsManifest.assets
        .filter(asset => offlineAssetsInclude.some(pattern => pattern.test(asset.url)))
        .filter(asset => !offlineAssetsExclude.some(pattern => pattern.test(asset.url)))
        .map(asset => new Request(asset.url, { integrity: asset.hash, cache: 'no-cache' }));
    await caches.open(cacheName).then(cache => cache.addAll(assetsRequests));

    // Take over right away instead of waiting until all tabs of the app are closed
    await self.skipWaiting();
}

async function onActivate(event) {
    console.info('Service worker: Activate');

    // Delete unused caches
    const cacheKeys = await caches.keys();
    await Promise.all(cacheKeys
        .filter(key => key.startsWith(cacheNamePrefix) && key !== cacheName)
        .map(key => caches.delete(key)));

    await self.clients.claim();
}

async function onFetch(event) {
    const cache = await caches.open(cacheName);

    // Hashed framework files: cache first, a new deploy references new file names
    if (immutableAssets.test(new URL(event.request.url).pathname)) {
        return (await cache.match(event.request)) || fetch(event.request);
    }

    // Everything else: network first so a simple refresh picks up a new deploy,
    // the cache is only the offline fallback
    const isNavigation = event.request.mode === 'navigate';
    try {
        // Navigation requests can't be cloned with a custom cache mode, fetch them by URL
        const response = await fetch(isNavigation ? event.request.url : event.request, { cache: 'no-cache' });
        return isNavigation && response.redirected ? Response.redirect(response.url) : response;
    } catch (error) {
        const cachedResponse = await cache.match(isNavigation ? 'index.html' : event.request);
        if (cachedResponse) {
            return cachedResponse;
        }
        throw error;
    }
}
