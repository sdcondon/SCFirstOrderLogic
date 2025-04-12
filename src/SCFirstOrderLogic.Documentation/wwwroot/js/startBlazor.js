// Shamelessly nabbed and adapted from https://swharden.com/blog/2022-05-29-blazor-loading-progress/
let loadedCount = 0;
const resourcesToLoad = [];
Blazor.start({
    loadBootResource:
        function (type, filename, defaultUri, integrity) {
            if (type == "dotnetjs")
                return defaultUri;

            const fetchResources = fetch(defaultUri, {
                cache: 'no-cache',
                integrity: integrity
            });

            resourcesToLoad.push(fetchResources);

            fetchResources.then((r) => {
                loadedCount += 1;
                if (filename == "blazor.boot.json")
                    return;
                const totalCount = resourcesToLoad.length;
                document.getElementById('progressLabel').innerHTML = `Loading,&nbsp;please&nbsp;wait..&nbsp;(${loadedCount}/${totalCount})`;
                document.getElementById('progressDetail').innerText = filename;
            });

            return fetchResources;
        }
});
