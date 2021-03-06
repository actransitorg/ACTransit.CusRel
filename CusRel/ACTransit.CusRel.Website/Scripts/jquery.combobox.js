/*!
 * Combobox Plugin for jQuery, version 0.5.0
 *
 * Copyright 2012, Dell Sala
 * http://dellsala.com/
 * https://github.com/dellsala/Combo-Box-jQuery-Plugin
 * Dual licensed under the MIT or GPL Version 2 licenses.
 * http://jquery.org/license
 *
 * Date: 2012-01-15
 */
(function () {
    var COMBOBOX_DATA_KEY = 'jQueryCombobox';
    var EVENT_NAMESPACE = '.jQueryCombobox';
    
    jQuery.combobox = {
        instances: []
    };

    jQuery.fn.combobox = function (selectOptions) {
        var el, combobox;

        // Destroy comboboxes
        if (selectOptions === 'destroy') {
            return this.each(function () {
                el = jQuery(this);
                combobox = el.data(COMBOBOX_DATA_KEY);
                for (var i = array.length - 1; i--;)
                    if (array[i] === combobox) array.splice(i, 1);
                combobox.destroy();
                el.removeData(COMBOBOX_DATA_KEY);

            });
        }

        return this.each(function () {
            combobox = new Combobox(this, selectOptions);
            jQuery.combobox.instances.push(combobox);
            jQuery(this).data(COMBOBOX_DATA_KEY, combobox);
        });

    };

    var Combobox = function (textInputElement, selectOptions) {
        this.textInputElement = jQuery(textInputElement);
        this.selector = new ComboboxSelector(this);
        this.setSelectOptions(selectOptions);
        if (this.textInputElement.parent().prop('className') == "combobox") {
            return;
        } 
        this.textInputElement.wrap(
            '<span class="combobox" style="position:relative; ' +
            'display:-moz-inline-box; display:inline-block;"/>'
        );
        this.selector = new ComboboxSelector(this);
        this.setSelectOptions(selectOptions);
        var inputHeight = this.textInputElement.outerHeight();
        var buttonLeftPosition = this.textInputElement.outerWidth() + 0;
        this.showSelectorButton = jQuery(
            '<a href="#" class="combobox_button combobox_glow" ' +
            'style="position:absolute; height:' + inputHeight + 'px; width:' +
            inputHeight + 'px; top:0; left:' + buttonLeftPosition + 'px;"><div class="combobox_arrow"></div></a>'
        );
        this.textInputElement.css('margin', '0 ' + this.showSelectorButton.outerWidth() + 'px 0 0').css('border-right-width', '0').css('border-radius', '4px 0px 0px 4px');
        this.showSelectorButton.insertAfter(this.textInputElement);
        var thisSelector = this.selector;
        var thisCombobox = this;
        this.showSelectorButton.bind('click' + EVENT_NAMESPACE, function (e) {
            e.preventDefault();
            e.stopPropagation();
            if (thisCombobox.isDisabled() === false && thisSelector.isVisible() === false) {
                thisSelector.buildSelectOptionList();
                thisSelector.show();
                thisCombobox.focus();
            } else {
                thisSelector.hide();
            }
            return false;
        });
        this.bindKeypress();
    };

    Combobox.prototype = {

        setSelectOptions: function (selectOptions) {
            this.selector.setSelectOptions(selectOptions);
            this.selector.buildSelectOptionList(this.getValue());
        },

        bindKeypress: function () {
            var thisCombobox = this;
            this.textInputElement.bind('keyup' + EVENT_NAMESPACE, function (event) {
                if (event.which == Combobox.keys.TAB
                    || event.which == Combobox.keys.SHIFT)  {
                    return;
                }
                if (event.which != Combobox.keys.DOWNARROW
                    && event.which != Combobox.keys.UPARROW
                    && event.which != Combobox.keys.ESCAPE
                    && event.which != Combobox.keys.ENTER) {
                    thisCombobox.selector.buildSelectOptionList(thisCombobox.getValue());
                }
                if (event.which === Combobox.keys.ENTER
                    || event.which === Combobox.keys.CTRL
                    || event.metaKey === true
                    || event.ctrlKey === true) {
                    return;
                }
                thisCombobox.selector.show();
            });
        },

        setValue: function (value) {
            var oldValue = this.textInputElement.val();
            this.textInputElement.val(value);
            if (oldValue != value) {
                this.textInputElement.trigger('change');
            }
        },

        getValue: function () {
            return this.textInputElement.val();
        },

        focus: function () {
            this.textInputElement.trigger('focus');
        },

        isDisabled : function() {
            return this.textInputElement.prop('disabled');
        },

        destroy : function() {
            this.textInputElement.unbind(EVENT_NAMESPACE);
            this.showSelectorButton.unbind(EVENT_NAMESPACE);
            this.selector.destroy();
        }

};

Combobox.keys = {
    UPARROW: 38,
    DOWNARROW: 40,
    ENTER: 13,
    CTRL : 17,
    ESCAPE: 27,
    TAB: 9,
    SHIFT: 16
};



var ComboboxSelector = function (combobox) {
    this.combobox = combobox;
    this.optionCount = 0;
    this.selectedIndex = -1;
    this.allSelectOptions = [];
    var selectorTop = combobox.textInputElement.outerHeight();
    var selectorWidth = combobox.textInputElement.outerWidth() + 34;
    this.selectorElement = jQuery(
        '<div class="combobox_selector" ' +
        'style="display:none; top: ' + selectorTop + 'px; width: ' + selectorWidth + 'px;"></div>'
    ).insertAfter(this.combobox.textInputElement);
    var thisSelector = this;
    this.keypressHandler = function (e) {
        if (e.which == Combobox.keys.DOWNARROW) {
            thisSelector.selectNext();
        } else if (e.which == Combobox.keys.UPARROW) {
            thisSelector.selectPrevious();
        } else if (e.which == Combobox.keys.ESCAPE) {
            thisSelector.hide();
            thisSelector.combobox.focus();
        } else if (e.which == Combobox.keys.ENTER) {
            if (thisSelector.selectedIndex !== -1) {
                e.preventDefault();
            }
            thisSelector.combobox.setValue(thisSelector.getSelectedValue());
            thisSelector.combobox.focus();
            thisSelector.hide();
        } else if (e.which == Combobox.keys.TAB) {
            thisSelector.hide();
        }
    }

};


ComboboxSelector.prototype = {

    setSelectOptions: function (selectOptions) {
        this.allSelectOptions = selectOptions;
    },

    buildSelectOptionList: function (startingLetters) {
        if (!startingLetters) {
            startingLetters = "";
        }
        this.unselect();
        this.selectorElement.empty();
        var selectOptions = [];
        this.selectedIndex = -1;
        var i;
        for (i = 0; i < this.allSelectOptions.length; i++) {
            if (!startingLetters.length
                || this.allSelectOptions[i].toLowerCase().indexOf(startingLetters.toLowerCase()) === 0) {
                selectOptions.push(this.allSelectOptions[i]);
            }
        }
        this.optionCount = selectOptions.length;
        var ulElement = jQuery('<ul></ul>').appendTo(this.selectorElement);
        for (i = 0; i < selectOptions.length; i++) {
            ulElement.append(jQuery('<li />', { text: selectOptions[i] }));
        }
        var thisSelector = this;
        this.selectorElement.find('li').click(function (e) {
            thisSelector.hide();
            thisSelector.combobox.setValue(jQuery(this).text());
            thisSelector.combobox.focus();
        });
        this.selectorElement.mouseover(function (e) {
            thisSelector.unselect();
        });
        this.htmlClickHandler = function () {
            thisSelector.hide();
        };

    },

    show: function () {
        if (this.selectorElement.find('li').length < 1
            || this.selectorElement.is(':visible')) {
            return false;
        }
        jQuery('html').bind('keydown' + EVENT_NAMESPACE,this.keypressHandler);
        this.selectorElement.slideDown('fast');
        jQuery('html').bind('click' + EVENT_NAMESPACE, this.htmlClickHandler);
        this.visible = true;
        return true;
    },

    hide: function () {
        jQuery('html').unbind('keydown' + EVENT_NAMESPACE, this.keypressHandler);
        jQuery('html').unbind('click' + EVENT_NAMESPACE, this.htmlClickHandler);
        this.selectorElement.unbind('click');
        this.unselect();
        this.selectorElement.hide();
        this.visible = false;
    },

    isVisible : function () {
        return this.visible === true;			
    },

    selectNext: function () {
        var newSelectedIndex = this.selectedIndex + 1;
        if (newSelectedIndex > this.optionCount - 1) {
            newSelectedIndex = this.optionCount - 1;
        }
        this.select(newSelectedIndex);
    },

    selectPrevious: function () {
        var newSelectedIndex = this.selectedIndex - 1;
        if (newSelectedIndex < 0) {
            newSelectedIndex = 0;
        }
        this.select(newSelectedIndex);
    },

    select: function (index) {
        this.unselect();
        this.selectorElement.find('li:eq(' + index + ')').addClass('selected');
        this.selectedIndex = index;
    },

    unselect: function () {
        this.selectorElement.find('li').removeClass('selected');
        this.selectedIndex = -1;
    },

    getSelectedValue: function () {
        if (this.selectedIndex !== -1) {
            return jQuery(this.selectorElement.find('li').get(this.selectedIndex)).text();
        } else {
            return this.combobox.textInputElement.val();
        }
    },

    destroy : function() {
        jQuery('html').unbind(EVENT_NAMESPACE);
    }

};


})();