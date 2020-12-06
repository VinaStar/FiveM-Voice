/*
 __      __   _          
 \ \    / /  (_)         
  \ \  / /__  _  ___ ___ 
   \ \/ / _ \| |/ __/ _ \
    \  / (_) | | (_|  __/
     \/ \___/|_|\___\___|
                         
*/

(function() {
	
	// Listen to game
	window.addEventListener("message", function(event) {
		
		var action = event.data.action;
		var data = event.data.data;
		
		var ignoreActions = ["UpdateTalking"];
		
		if (!ignoreActions.includes(action))
			console.log("Voice received message from game: ", action, data);
			
		switch(action) {
			case "SetEnabled":
				SetEnabled((data == "True"));
				break;
			
			case "SetVisibility":
				SetVisibility((data == "True"));
				break;
				
			case "SetVoiceLevel":
				SetVoiceLevel(data);
				break;
				
			case "SetChannel":
				SetChannel(parseInt(data));
				break;
				
			case "UpdateTalking":
				UpdateTalking(data);
				break;
		}
		
	});
	
	function getContainer() {
		return $("ui");
	}
	
	var isEnabled = false;
	var isVisible = false;
	var level = "";
	var channel = -1;
	
	/*
	    _   ___ _____ ___ ___  _  _ ___ 
	   /_\ / __|_   _|_ _/ _ \| \| / __|
	  / _ \ (__  | |  | | (_) | .` \__ \
	 /_/ \_\___| |_| |___\___/|_|\_|___/
	 
	*/
	
	function SetEnabled(enabled) {
		isEnabled = enabled;
		var container = getContainer();
		if (isEnabled && isVisible) container.addClass("visible");
		else container.removeClass("visible");
	}
	
	function SetVisibility(visible) {
		isVisible = visible;
		var container = getContainer();
		if (isEnabled && isVisible) container.addClass("visible");
		else container.removeClass("visible");
	}
	
	function SetVoiceLevel(voiceLevel) {
		level = voiceLevel;
		if (channel <= -1)
			getContainer().find(".voiceLevel").html(level);
	}
	
	function SetChannel(channel) {
		if (channel > -1)
			getContainer().find(".voiceChannel").html("Channel: " + channel);
	}
	
	function UpdateTalking(talkings) {
		var talkingContainer = getContainer().find(".talkings-container");
		if (talkings == ""){
			$(talkingContainer).removeClass("visible");
		}
		else {
			$(talkingContainer).addClass("visible");
		}
		
		getContainer().find(".talkings").html(talkings);
	}
	
})();