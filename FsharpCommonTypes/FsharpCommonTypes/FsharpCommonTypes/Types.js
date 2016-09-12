(function (global, factory) {
  if (typeof define === "function" && define.amd) {
    define(["exports", "fable-core"], factory);
  } else if (typeof exports !== "undefined") {
    factory(exports, require("fable-core"));
  } else {
    var mod = {
      exports: {}
    };
    factory(mod.exports, global.fableCore);
    global.Types = mod.exports;
  }
})(this, function (exports, _fableCore) {
  "use strict";

  Object.defineProperty(exports, "__esModule", {
    value: true
  });
  exports.PropertyError = exports.SummaryError = undefined;

  function _classCallCheck(instance, Constructor) {
    if (!(instance instanceof Constructor)) {
      throw new TypeError("Cannot call a class as a function");
    }
  }

  var _createClass = function () {
    function defineProperties(target, props) {
      for (var i = 0; i < props.length; i++) {
        var descriptor = props[i];
        descriptor.enumerable = descriptor.enumerable || false;
        descriptor.configurable = true;
        if ("value" in descriptor) descriptor.writable = true;
        Object.defineProperty(target, descriptor.key, descriptor);
      }
    }

    return function (Constructor, protoProps, staticProps) {
      if (protoProps) defineProperties(Constructor.prototype, protoProps);
      if (staticProps) defineProperties(Constructor, staticProps);
      return Constructor;
    };
  }();

  var SummaryError = exports.SummaryError = function () {
    function SummaryError(errorCode, description) {
      _classCallCheck(this, SummaryError);

      this.ErrorCode = errorCode;
      this.Description = description;
    }

    _createClass(SummaryError, [{
      key: "Equals",
      value: function Equals(other) {
        return _fableCore.Util.equalsRecords(this, other);
      }
    }, {
      key: "CompareTo",
      value: function CompareTo(other) {
        return _fableCore.Util.compareRecords(this, other);
      }
    }]);

    return SummaryError;
  }();

  _fableCore.Util.setInterfaces(SummaryError.prototype, ["FSharpRecord", "System.IEquatable", "System.IComparable"], "CommonValidations.SummaryError");

  var PropertyError = exports.PropertyError = function () {
    function PropertyError(errorCode, description, propertyName) {
      _classCallCheck(this, PropertyError);

      this.ErrorCode = errorCode;
      this.Description = description;
      this.PropertyName = propertyName;
    }

    _createClass(PropertyError, [{
      key: "Equals",
      value: function Equals(other) {
        return _fableCore.Util.equalsRecords(this, other);
      }
    }, {
      key: "CompareTo",
      value: function CompareTo(other) {
        return _fableCore.Util.compareRecords(this, other);
      }
    }]);

    return PropertyError;
  }();

  _fableCore.Util.setInterfaces(PropertyError.prototype, ["FSharpRecord", "System.IEquatable", "System.IComparable"], "CommonValidations.PropertyError");
});