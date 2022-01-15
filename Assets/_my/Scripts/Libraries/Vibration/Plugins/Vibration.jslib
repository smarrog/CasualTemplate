mergeInto(LibraryManager.library, {

  is_vibration_enabled_js: function() {
    return typeof navigator.vibrate === 'function';
  },

  vibrate_pattern_js: function(arrayPointer, length) {
    var arr = [];
    for (var i = 0; i < length; i++) {
      arr.push(parseInt(Module.HEAP32[(arrayPointer >> 2) + i]));
    }
    navigator.vibrate(arr);
  },
  
  vibrate_single_js: function (durationInMs) {
    navigator.vibrate(durationInMs);
  }
  
});