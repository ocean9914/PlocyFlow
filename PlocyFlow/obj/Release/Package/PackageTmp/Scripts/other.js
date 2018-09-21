function OpenNewWin(Url, w, h)
{
    var width = w;
    var height = h;
    open(
			Url,
			'main',
			'width='
					+ width
					+ ',height='
					+ height
					+ ',left=20,top=60,menubar=no,toolbar=no,location=no,scrollbars=yes,status=yes,resizable=yes');
}