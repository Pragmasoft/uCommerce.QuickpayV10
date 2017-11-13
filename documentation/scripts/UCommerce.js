$('a:has(".toggle")').css('padding-left', '0');

$(function() {	
	$(".accordion a").each(function() {
		var match = location.href.toLowerCase().match(".*documentation\/")
		if(match == null) 
		{
			this.href = "/" + this.getAttribute("href"); 
		} 
		else  
		{
			this.href = match + this.getAttribute("href"); 
		}
	});
});

$(function() {	
	$(".main-col a").each(function() {
		var r = new RegExp('^(?:[a-z]+:)?//', 'i');
		if(!r.test(this.getAttribute("href"))) {
			var match = location.href.toLowerCase().match(".*documentation\/")
			if(match == null) 
			{
				this.href = "/" + this.getAttribute("href"); 
			} 
			else  
			{
				this.href = match + this.getAttribute("href"); 
			}
		}
	});
});

$(function() {	
	$(".banner img.banner-image").each(function() {
		var match = location.href.toLowerCase().match(".*documentation\/")
		if(match == null) 
		{
			this.src = "/" + this.getAttribute("src"); 
		} 
		else  
		{
			this.src = match + this.getAttribute("src"); 
		}
	});
});

$(function() {
	$('.accordion').children().each(function(i, el) { 
		var fullPath = $(el).children()[0].href;
		var slug = fullPath.replace(/\/[^\/]*$/, '');

		if(location.href.indexOf(slug) >= 0) {
			// Selected section
			$($(el).children()[1]).css('display', '');
			$($(el).children()[0]).attr('class','active');
			
			SetSelectedDocument(el);
		}
	});
	
	$('#versionSelector').change(function() {
		window.location.replace($(this).find(":selected").val());
	});
});

function SetSelectedDocument(el) {
	if($(el).children()[0].href == location.href) {
		$($(el).children()[0]).attr('class','active');
		$($(el).children()[0]).attr('class','selected');
		
		if($(el).children()[1]) {
			$($(el).children()[1]).css('display', '');
		}
		
		$(el).parents('ul').each(function(i, parentEl) {
			$(parentEl).css('display','');
		});

		SetParentsActive(el);
	}

	var elChildren = $(el).children()[1];
	
	if(elChildren) {
		$(elChildren).children().each(function(i, childEl) {
			SetSelectedDocument(childEl);
		});
	}
}

//Add 'active' style class to all parent folders
function SetParentsActive(el) {	
	var parent = $(el).parent();
	if(parent) {
		$(parent).children('a').each(function(i, child) {
			$(child).attr('class','active');
		});

		if(!el.parentElement) return;
		SetParentsActive(parent)
	}
}

function plusone_vote(obj)
{ try { pageTracker._trackEvent('GooglePlus', 'Like', obj.state); } catch (err) { } }

(function () {
	var po = document.createElement('script'); po.type = 'text/javascript'; po.async = true;
	po.src = 'https://apis.google.com/js/plusone.js';
	var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(po, s);
})();