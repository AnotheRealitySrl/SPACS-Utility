var plugin = {
  SaveDataToSessionStorage: function (key, value) {
    var keyStr = UTF8ToString(key);
    var valueStr = UTF8ToString(value);
    // L'unica differenza è qui:
    window.sessionStorage.setItem(keyStr, valueStr); 
  },

  LoadDataFromSessionStorage: function (key) {
    var keyStr = UTF8ToString(key);
    // E qui:
    var value = window.sessionStorage.getItem(keyStr); 
    
    if (value === null) {
        return null;
    }
    var bufferSize = lengthBytesUTF8(value) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(value, buffer, bufferSize);
    return buffer;
  },
};

mergeInto(LibraryManager.library, plugin);
