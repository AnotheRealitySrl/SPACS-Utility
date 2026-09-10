mergeInto(LibraryManager.library, {
  // Funzione che recupera la querystring dal browser
  GetQueryString: function () {
    // window.location.search restituisce la parte dell'URL dal '?' in poi
    var returnStr = window.location.search;

    // Alloca memoria per la stringa da passare a C#
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);

    // Ritorna il puntatore alla stringa in memoria
    return buffer;
  }
});