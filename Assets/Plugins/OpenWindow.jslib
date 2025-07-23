mergeInto(LibraryManager.library, {
	OpenWindow: function (url) {
		url = UTF8ToString(url);
		window.open(url);
    }
});