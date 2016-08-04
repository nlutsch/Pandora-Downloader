$( function() {
    // Info Box
    $('body').append('<div class="pandora-downloader-infobox"></div>')

    function display_info(message) {
        $('.pandora-downloader-infobox')
            .finish()
            .fadeOut( 10 )
            .text( message )
            .fadeIn( 400 )
            .delay( 10000 )
            .fadeOut( 400 );
    }

    //Messages from background.js
    chrome.runtime.onMessage.addListener( function(request, sender, sendResponse) {
        //Sent when new song starts
        if (request == '') 
	        get_info();

        //Status update to show on page
        else
            display_info(request);
    });

    function get_info() {
        //Wait for track information to load
        setTimeout( function () {
            chrome.runtime.sendMessage({
                title:   $('.playerBarSong').text(),
                artist:  $('.playerBarArtist').text(),
                album:   $('.playerBarAlbum').text(),
	            station: $('.stationChangeSelector').find('.textWithArrow').find('p').text(),
	            artUrl:  $($('.slide')[1]).find('.art').attr('src')
            }, function( response ) {} );
        }, 5000);
    }
})
