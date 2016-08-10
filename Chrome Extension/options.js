// Saves options to chrome.storage
	function save_options() {
	  var newPort = document.getElementById('inPort').value;
	  chrome.storage.sync.set({
		port: newPort
	  }, function() {
		// Update status to let user know options were saved.
		var status = document.getElementById('status');
		status.textContent = 'Options saved.';
		setTimeout(function() {
		  status.textContent = '';
		}, 2000);
	  });
	}

	// Restores select box and checkbox state using the preferences
	// stored in chrome.storage.
	function restore_options() {
	  chrome.storage.sync.get({
		port: 2002,
		likesColor: true
	  }, function(items) {
		document.getElementById('inPort').value = items.port;
	  });
	}

	document.addEventListener('DOMContentLoaded', restore_options);
	document.getElementById('save').addEventListener('click', save_options);