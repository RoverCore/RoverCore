﻿// equal heights in jQuery, see https://github.com/mattbanks/jQuery.equalHeights
!function (a) { a.fn.equalHeights = function () { var b = 0, c = a(this); return c.each(function () { var c = a(this).innerHeight(); c > b && (b = c) }), c.css("height", b) }, a("[data-equal]").each(function () { var b = a(this), c = b.data("equal"); b.find(c).equalHeights() }) }(jQuery);
