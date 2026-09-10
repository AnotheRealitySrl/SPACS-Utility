var plugin = {
  OpenLink: function (url) {
    url = UTF8ToString(url);
    const wh = window.open(url, "backOffice", "_self");
    if (wh) {
      wh.focus();
      return true;
    } else {
      return false;
    }
  },

  DownloadFile: function (array, size, fileNamePtr) {
    var fileName = UTF8ToString(fileNamePtr);

    var bytes = new Uint8Array(size);
    for (var i = 0; i < size; i++) {
      bytes[i] = HEAPU8[array + i];
    }

    var blob = new Blob([bytes]);
    var link = document.createElement("a");
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;

    var event = document.createEvent("MouseEvents");
    event.initMouseEvent("click");
    link.dispatchEvent(event);
    window.URL.revokeObjectURL(link.href);
  },
};
mergeInto(LibraryManager.library, plugin);
