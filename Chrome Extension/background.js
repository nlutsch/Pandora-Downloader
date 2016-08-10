$( function() {
    var currentSongUrl = undefined;
	var port = 2002;
	chrome.storage.sync.get('port', function(obj) {
		port = obj.port;
	});

    // Setup the listener for pandora's outgoing requests
    chrome.webRequest.onBeforeRequest.addListener( function(details) {
        if ( details.url.match( /(http.*\.pandora.com\/access\/.*)/i ) || details.url.match( /(http.*\.cdn.com\/access\/.*)/i )) {
            currentSongUrl = details.url;
	        setTimeout(function () {
                sendMessageToTabs();
            }, 5000);
        }   
    },
    { urls: ['<all_urls>'] });

    //Messages from content.js with track information
    chrome.runtime.onMessage.addListener( function(request, sender, sendResponse) {
	    download(request, sender, sendResponse);
    });

    function send_info(message) {
        var d = new Date();
        console.log( d.toLocaleString() + ' - ' + message );
        sendMessageToTabs(message);
    }

    function download(request, sender, sendResponse) {
        var song = '\'' + request.artist + ' - ' + request.title + '\'';		

        // The information isn't available yet
        if ( request.title == "" ) {
            if ($.isReady) {
                return 2;
            }
            else {
                send_info('The song info wasn\'t passed in!');
                
                setTimeout(function () {
                    sendMessageToTabs();
	            	download(request, sender, sendResponse);
	        	}, 2000);   
            }      
        }

        // No song url has been captured
        if ( currentSongUrl === undefined ) {
            send_info('The song url hasn\'t been captured! Skip to the next song, and get this one later.');
            setTimeout(function () {
                sendMessageToTabs();
	            download(request, sender, sendResponse);
	        }, 2000);  
        }

		var requestUrl = 'http://127.0.0.1:' + port + '/Download/Download';
		console.log(requestUrl);
		
        $.ajax({
            type: 'POST',
            url: requestUrl,
            data: {
                url   	 : currentSongUrl,
                title 	 : request.title,
                artist	 : request.artist,
                album 	 : request.album,
		        station  : request.station,
		        artUrl   : request.artUrl
            },
            async: false
        }).success(function(request, textStatus, xhr) {
            if (request.status == "Already Exists") {
                send_info(song + ' has already been downloaded!')
            }
            else {
                send_info(song + ' is downloading.')
            }
        }).error(function (request, textStatus, xhr) {
            if (xhr.status == 404)
                send_info('The web server isn\'t running!');
			else if (xhr.status == 400)
				send_info('Could not find or create directory set in appsettings.config');
            else if (xhr.status == 500)
                send_info('There was an internal web server error');
            else
                send_info('An unknown error occured. Check that your ports match.');
        });
        return 1;       
    }

    function sendMessageToTabs(message) {
        if (message == undefined) {
            message = '';
        }
	    chrome.tabs.query({}, function(tabs) {
	        for (var i=0; i<tabs.length; ++i) {
        	    chrome.tabs.sendMessage(tabs[i].id, message);
    	    }
	    });
    }
	
	function saveSong(songUrl, title, artist, album, station, artUrl) {
		
	}
})