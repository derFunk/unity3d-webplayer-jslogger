/**
** Unity Logger Inject Script
** Do not use single line comments!
**/
/***************************************************************************\
Project:      Javascript Logger for Unity3D Webplayer
Copyright (c) Andreas Katzig, Chimera Entertainment GmbH

THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

window.unityLogScrollingEnabled = true;

var unityLogEl = document.getElementById('unity_log');
var unityLogStopBtn = document.getElementById('unity_log_stop_button');
var unityLogTypes = new Array('TRACE', 'INFO','DEBUG','WARN', 'ERROR', 'FATAL');
var unityPlayerEl = null;
var toggleBtnTxt = 'Switch log scrolling';

if (typeof consoleLogEnabled == 'undefined')
	var consoleLogEnabled = false;

if (typeof unity_log_max_entries == 'undefined')
	var unity_log_max_entries = 500;

function doubleZero(intgr) {
	if (intgr < 10) return ('0'.concat(String(intgr)));
	return intgr;
}

function consoleLog(str) {
	if (!consoleLogEnabled) return;

	if (typeof console != 'undefined' && typeof console.log == 'function')
		console.log(str);
}

function toggleVisibility(obj) {
    if (obj == null || obj.style == null || typeof obj.style == 'undefined')
        return false;

    if (obj.style.display.substring(0, 4) == 'none')
        obj.style.display = '';
    else
        obj.style.display = 'none';
        
    return true;
}


function toggleUnityLogFilter(linkObj, logtype)
{
    var rule = getCSSRule('.unity_log_entry_' + logtype);
	
	if (rule && linkObj.style.textDecoration == ''){
		linkObj.style.textDecoration = 'line-through';
	}
    else if (rule) {
		linkObj.style.textDecoration = '';
	}

    toggleVisibility(rule);	

	scrollUnityLogToBottom();
	
	consoleLog('Toggle Log Filter for ' + logtype);
}


function addCSSRule(selector, rule) {
	if (document.styleSheets) {
		if (document.styleSheets[0].cssRules) {
			try{
				return document.styleSheets[0].insertRule(selector + " { " + rule + " } " );
			}
			catch(err){
			}
		}
	}
	return false;
}


function getCSSRule(ruleName) {
	
    ruleName = ruleName.toLowerCase();

    consoleLog('Looking for CSS rule ' + ruleName);
	   
   if (document.styleSheets) {
	   for (var i = 0; i < document.styleSheets.length; i++) {
		   var styleSheet = document.styleSheets[i];

		   if (!styleSheet)
				continue;
		   consoleLog('Looking in Stylesheet #' + i);

		 var ii=0;
		 var cssRule=false;
		 do {
			if (styleSheet.cssRules) {
			   cssRule = styleSheet.cssRules[ii];
			} else {
                if (styleSheet.rules != null)
                    cssRule = styleSheet.rules[ii];
			}

		    if (cssRule) {
                
				if (cssRule.selectorText.toLowerCase() == ruleName) {
					// consoleLog('Found CSS rule ' + ruleName);
					return cssRule;
			   }
			}
			ii++;
		 } while (cssRule)
	  }
	}
   
   consoleLog('Didnt find for CSS rule ' + ruleName);
   
   return false;
}


function createFilterBar()
{
	var filterEl = document.getElementById('unity_log_filter');
	if (filterEl != null)
		return;
	
	filterEl = document.createElement('div');
	filterEl.setAttribute('id', 'unity_log_filter');

    // add filter level links/buttons
	for (var i in unityLogTypes)
	{
		filterEl.innerHTML += "<a href='#unity_log' title='Toggle all " + unityLogTypes[i] + " messages.' onclick='toggleUnityLogFilter(this, \"" + unityLogTypes[i].toLowerCase() + "\");'>" + unityLogTypes[i] + "</a> ";
	}
    
    // add save log link
	filterEl.innerHTML += '&nbsp;&nbsp;|&nbsp;&nbsp;<a href="#" onclick="downloadlog();return false;" download="WebPlayer_Log.txt">DOWNLOAD LOGFILE</a>';

	unityLogEl.parentNode.insertBefore(filterEl, unityLogEl.nextSibling);
}

function downloadlog() {
    var currentLogText = unityLogEl.textContent;
   
    var blob = new Blob([currentLogText], { type: "text/plain;charset=utf-8" });
    saveAs(blob, "WebPlayer_Log.txt");
}


function clearLogName(logname) {
    return logname.replace(/\./g, "_").toLowerCase();
}

function unityLog(logname, logtype, msg) {
	if (unityLogEl == null) {
		consoleLog('log does not exist!');
		return;
	}

	if (logname == '' || logtype == '' || msg == '') {
		consoleLog('invalid parameters at log call');
		return;
	}
	
	consoleLog('calling log() ' + logname);

	var date = new Date();
	/* date.getDate() + '.' + doubleZero((date.getMonth() + 1)) + '.' + date.getFullYear() + ' ' */
	var now = doubleZero(date.getHours()) + ':'
								+ doubleZero(date.getMinutes()) + ':'
								+ doubleZero(date.getSeconds()) + ':'
								+ doubleZero(date.getMilliseconds());

	/* check length of log */
	var lines = unityLogEl.innerHTML.split('<pre');

	if (lines.length > unity_log_max_entries) {
		consoleLog('Cropping log to ' + unity_log_max_entries + ' lines');
		var buf = '';
		buf = lines.slice((lines.length - unity_log_max_entries + 1), lines.length);
		consoleLog('buf is ' + buf.length + ' lines');
		buf = buf.join('<pre');
		unityLogEl.innerHTML = '<pre' + buf;
	}

	var loggerNameStyle = "unity_log_name_" + clearLogName(logname);

	var loggerNameCSSRule = getCSSRule("." + loggerNameStyle);

	if (!loggerNameCSSRule)
		addCSSRule("." + loggerNameStyle, "display:");

	unityLogEl.innerHTML = unityLogEl.innerHTML + '<pre class="' + loggerNameStyle + ' unity_log_entry unity_log_entry_alt' + ((lines.length % 2) + 1)
										+ ' unity_log_entry_' + logtype.toLowerCase() + '">'
										+ now + '\t'
										+ logtype + '\t'
										/* TODO + '<a href="#unity_log" class="unity_log_entry_' + logtype.toLowerCase() + '" onclick="toggleLogger(\'' + loggerNameStyle + '\')">' */
                                        + logname
                                        /* + '</a>' */
                                        + '\t' + msg + '</pre>\n';

	scrollUnityLogToBottom();
}

/* TODO
var logNameFilterActive = false;
function toggleLogger(loggerNameStyle) {
    var logEntryRule = getCSSRule(".unity_log_entry");
    toggleVisibility(logEntryRule);

    if (logNameFilterActive) {
        logNameFilterActive = false;
        return;
    }

    var logNameRule = getCSSRule("." + loggerNameStyle);
    if (logNameRule) {
        toggleVisibility(logNameRule);
        logNameFilterActive = true;
        consoleLog('Toggle visibility to visible: ' + logNameRule);
    }

}
*/

function scrollUnityLogToBottom()
{
	if (window.unityLogScrollingEnabled)
		unityLogEl.scrollTop = unityLogEl.scrollHeight;
}


/* This function must be safe to be callable multiple times! */
function ensureLog() {
	if (unityLogEl != null) {
		consoleLog('log div already exists!');
		return; /* already exists */
	}

	consoleLog('calling ensureLog()');

	unityPlayerEl = document.getElementById('unityPlayer');
	
	if (unityPlayerEl == null) {
		consoleLog('player div not found!');
		return; /* no player, no log */
	}

	/* add default jslog implementation */
	appendDefaultLogStyle();

	if (createUnityLog())
	{
		createFilterBar();
	}
}


function createUnityLog()
{
	if (unityLogEl != null) {
		return false; /* already exists */
	}
	
	unityLogEl = document.createElement('div');
	unityLogEl.setAttribute('id', 'unity_log');

	unityPlayerEl.parentNode.insertBefore(unityLogEl, unityPlayerEl.nextSibling);

	if (unityLogStopBtn == null) {
		unityLogStopBtn = document.createElement('input');
		unityLogStopBtn.setAttribute('type', 'button');
		unityLogStopBtn.setAttribute('onclick', 'toggleLogScrolling();');
		unityLogStopBtn.setAttribute('value', toggleBtnTxt + " off");
		unityLogStopBtn.setAttribute('id', 'unity_log_toggle');
		unityLogEl.parentNode.insertBefore(unityLogStopBtn, unityLogEl.nextSibling);
	}
	
	return true;
}

function toggleLogScrolling()
{
	window.unityLogScrollingEnabled = !window.unityLogScrollingEnabled;
	document.getElementById("unity_log_toggle").value = toggleBtnTxt + " " + ((!window.unityLogScrollingEnabled) ? "on" : "off");
}

function getDefaultLogStyles() {
    var styles = '#unity_log { background-color: #000; font-size: 11pt; height: 250px; width: 100%; border: 1px solid grey; overflow: auto; text-align: left;  resize: both; }\n';
	styles += '.unity_log_entry_debug { color: lightgreen; }\n';
	styles += '.unity_log_entry_info { color: lightblue; font-weight: bold }\n';
	styles += '.unity_log_entry_error { color: red; font-weight: normal }\n';
	styles += '.unity_log_entry_warn { color: orange; font-weight: bold }\n';
	styles += '.unity_log_entry_trace { color: yellow; font-weight: bold }\n';
	styles += '.unity_log_entry_fatal { color: red; font-weight: bold }\n';
	styles += '.unity_log_entry { margin: 0; font-size: 8pt; }\n';
	/* styles += '.unity_log_entry_alt1 { background-color: #F8F8FF; border: 1px solid #F0F0FF; }\n'; */
	styles += '.unity_log_entry_alt1 { background-color: #101010; border: 1px solid #181818; }\n';
	styles += '.unity_log_entry_alt2 { background-color: #101030 }\n';
	return styles;
}

function appendDefaultLogStyle() {
	var headEl = document.getElementsByTagName('head');
	if (headEl.length == 0 || typeof headEl == 'undefined') // type "object" in chrome, "function" in safari
	{
		consoleLog('Could not apply log styles, head-element not found!');
		return;
	}
    
	var css = document.createElement('style');
	css.type = 'text/css';

	if (css.styleSheet)
	    css.styleSheet.cssText = getDefaultLogStyles();
	else
	    css.appendChild(document.createTextNode(getDefaultLogStyles()));

	if (typeof document.getElementsByTagName('head')[0] == 'object')
		document.getElementsByTagName('head')[0].appendChild(css);
}


/* IMPORTANT: CALL HERE for init*/
ensureLog();