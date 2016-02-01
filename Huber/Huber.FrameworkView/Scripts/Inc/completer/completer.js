/*!
 * Completer v0.1.2
 * https://github.com/fengyuanchen/completer
 *
 * Copyright 2014-2015 Fengyuan Chen
 * Released under the MIT license
 *
 * Date: 2015-01-03T12:17:43.862Z
 */

(function (factory) {
  if (typeof define === 'function' && define.amd) {
    // AMD. Register as anonymous module.
    define(['jquery'], factory);
  } else if (typeof exports === 'object') {
    // Node / CommonJS
    factory(require('jquery'));
  } else {
    // Browser globals.
    factory(jQuery);
  }
})(function ($) {

  'use strict';

  var $window = $(window),
      $document = $(document),
      Completer = function (element, options) {
        this.$element = $(element);
        this.defaults = $.extend({}, Completer.defaults, this.$element.data(), $.isPlainObject(options) ? options : {});
        this.init();
      },

      toRegexp = function (s) {
        if (typeof s === 'string' && s !== '') {
          s = espace(s);

          return new RegExp(s + '+[^' + s + ']*$', 'i');
        }

        return null;
      },

      espace = function (s) {
        return s.replace(/([\.\$\^\{\[\(\|\)\*\+\?\\])/g, '\\$1');
      },

      toArray = function (s) {
        if (typeof s === 'string') {
          s = s.replace(/[\{\}\[\]"']+/g, '').split(/\s*,+\s*/);
        }

        s = $.map(s, function (n) {
          return typeof n !== 'string' ? n.toString() : n;
        });

        return s;
      };

  Completer.prototype = {
    constructor: Completer,

    init: function () {
      var defaults = this.defaults,
          data = toArray(defaults.source);

      if (data.length > 0) {
        this.data = data;
        this.regexp = toRegexp(defaults.separator);
        this.$completer = $(defaults.template);
        this.$completer.hide().appendTo('body');
        this.place();

        this.$element.attr('autocomplete', 'off').on({
          focus: $.proxy(this.enable, this),
          blur: $.proxy(this.disable, this)
        });

        if (this.$element.is(':focus')) {
          this.enable();
        }
      }
    },

    enable: function () {
      if (!this.active) {
        this.active = true;
        this.$element.on({
          keydown: $.proxy(this.keydown, this),
          keyup: $.proxy(this.keyup, this)
        });
        this.$completer.on({
          mousedown: $.proxy(this.mousedown, this),
          mouseover: $.proxy(this.mouseover, this)
        });
      }
    },

    disable: function () {
      if (this.active) {
        this.active = false;
        this.$element.off({
          keydown: this.keydown,
          keyup: this.keyup
        });
        this.$completer.off({
          mousedown: this.mousedown,
          mouseover: this.mouseover
        });
      }
    },

    attach: function (val) {
      var separator = this.defaults.separator,
          regexp = this.regexp,
          part = regexp ? val.match(regexp) : null,
          matched = [],
          all = [],
          that = this,
          reg,
          item;

      if (part) {
        part = part[0];
        val = val.replace(regexp, '');
        reg = new RegExp('^' +  espace(part), 'i');
      }

      $.each(this.data, function (i, n) {
        n = separator + n;
        item = that.template(val + n);

        if (reg && reg.test(n)) {
          matched.push(item);
        } else {
          all.push(item);
        }
      });

      matched = matched.length ? matched.sort() : all;

      if (this.defaults.position === 'top') {
        matched = matched.reverse();
      }

      this.fill(matched.join(''));
    },

    suggest: function (val) {
      var reg = new RegExp(espace(val), 'i'),
          that = this,
          matched = [];

      $.each(this.data, function (i, n) {
        if (reg.test(n)) {
          matched.push(n);
        }
      });

      matched.sort(function (a, b) {
        return a.indexOf(val) - b.indexOf(val);
      });

      $.each(matched, function (i, n) {
        matched[i] = that.template(n);
      });

      this.fill(matched.join(''));
    },

    template: function (text) {
      var tag = this.defaults.itemTag;

      return ('<' + tag + '>' + text + '</' + tag + '>');
    },

    fill: function (html) {
      var filter;

      this.$completer.empty();

      if (html) {
        filter = this.defaults.position === 'top' ? ':last' : ':first';
        this.$completer.html(html);
        this.$completer.children(filter).addClass(this.defaults.selectedClass);
        this.show();
      } else {
        this.hide();
      }
    },

    complete: function () {
      var defaults = this.defaults,
          val = defaults.filter(this.$element.val()).toString();

      if (val === '') {
        this.hide();
        return;
      }

      if (defaults.suggest) {
        this.suggest(val);
      } else {
        this.attach(val);
      }
    },

    keydown: function (e) {
      if (e.keyCode === 13) {
        e.stopPropagation();
        e.preventDefault();
      }
    },

    keyup: function (e) {
      var keyCode = e.keyCode;

      if (keyCode === 13 || keyCode === 38 || keyCode === 40) {
        this.toggle(keyCode);
      } else {
        this.complete();
      }
    },

    mouseover: function (e) {
      var defaults = this.defaults,
          selectedClass = defaults.selectedClass,
          $target = $(e.target);

      if ($target.is(defaults.itemTag)) {
        $target.addClass(selectedClass).siblings().removeClass(selectedClass);
      }
    },

    mousedown: function (e) {
      e.stopPropagation();
      e.preventDefault();
      this.setValue($(e.target).text());
    },

    setValue: function (val) {
      this.$element.val(val);
      this.defaults.complete();
      this.hide();
    },

    toggle: function (keyCode) {
      var selectedClass = this.defaults.selectedClass,
          $selected = this.$completer.find('.' + selectedClass);

      switch (keyCode) {

        // Down
        case 40:
          $selected.removeClass(selectedClass);
          $selected = $selected.next();
          break;

        // Up
        case 38:
          $selected.removeClass(selectedClass);
          $selected = $selected.prev();
          break;

        // Enter
        case 13:
          this.setValue($selected.text());
          break;

        // No default
      }

      if ($selected.length === 0) {
        $selected = this.$completer.children(keyCode === 40 ? ':first' : ':last');
      }

      $selected.addClass(selectedClass);
    },

    place: function () {
      var $element = this.$element,
          offset = $element.offset(),
          left = offset.left,
          top = offset.top,
          height = $element.outerHeight(),
          width = $element.outerWidth(),
          styles = {
            minWidth: width,
            zIndex: this.defaults.zIndex
          };

      switch (this.defaults.position) {
        case 'right':
          styles.left = left + width;
          styles.top = top;
          break;

        case 'left':
          styles.right = $window.innerWidth() - left;
          styles.top = top;
          break;

        case 'top':
          styles.left = left;
          styles.bottom = $window.innerHeight() - top;
          break;

        // case 'bottom':
        default:
          styles.left = left;
          styles.top = top + height;
      }

      this.$completer.css(styles);
    },

    show: function () {
      this.$completer.show();
      $window.on('resize', $.proxy(this.place, this));
      $document.on('mousedown', $.proxy(this.hide, this));
    },

    hide: function () {
      this.$completer.hide();
      $window.off('resize', this.place);
      $document.off('mousedown', this.hide);
    },

    destroy: function () {
      this.hide();
      this.disable();

      this.$element.off({
        focus: this.enable,
        blur: this.disable
      });
    }
  };

  Completer.defaults = {
    itemTag: 'li',
    position: 'bottom', // or 'right'
    source: [],
    selectedClass: 'completer-selected',
    separator: '',
    suggest: false,
    template: '<ul class="completer-container"></ul>',
    zIndex: 1,

    complete: $.noop,

    filter: function (val) {
      return val;
    }
  };

  Completer.setDefaults = function (options) {
    $.extend(Completer.defaults, options);
  };

  // Register as jQuery plugin
  $.fn.completer = function (options) {
    var args = [].slice.call(arguments, 1),
        result;

    this.each(function () {
      var $this = $(this),
          data = $this.data('completer'),
          fn;

      if (!data) {
        $this.data('completer', (data = new Completer(this, options)));
      }

      if (typeof options === 'string' && $.isFunction((fn = data[options]))) {
        result = fn.apply(data, args);
      }
    });

    return (typeof result !== 'undefined' ? result : this);
  };

  $.fn.completer.Constructor = Completer;
  $.fn.completer.setDefaults = Completer.setDefaults;

  $(function () {
    $('[data-toggle="completer"],[completer]').completer();
  });

});
