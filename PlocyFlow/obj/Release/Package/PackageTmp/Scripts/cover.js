/// <reference path="Scripts/jquery-1.4.2-vsdoc.js" />
/// <reference path="Scripts/MicrosoftAjax.debug.js" />



// 消息提示的类
var InfoTip = {};
// 显示消息的类型
InfoTip.types = [
    "note",
    "info",
    "success",
    "warning",
    "error"
];

InfoTip._timer = undefined;

// 显示消息
// @message:
// @type:
// @timeout: 消息显示的时长，该时间后自动隐藏。设置为0时不隐藏，默认2500毫秒。
InfoTip.showMessage = function (message, typeID, timeout) {
    if (timeout == undefined)
        timeout = 2500;

    if (InfoTip._timer) {
        clearTimeout(InfoTip._timer);
        InfoTip._timer = undefined;
    }
    $("#infoTipContainer").remove();

    var type = InfoTip.types[typeID];
    var note = $("<div id='infoTipContainer'><div><span class='close'></span><span class='text'>Info Tip</span></div></div>");

    note.attr("class", type);
    $(".text", note).html(message);
    $(document.body).append(note);

    //if (type == "note") {
    $(".close", note).show().click(function () {
        note.animate({ top: 102 }, "fast", null, function () { note.hide(); });
    });
    //}

    note.animate({ top: 122 }, "normal");

    if (timeout > 0) {
        InfoTip._timer = setTimeout(function () {
            note.animate({ top: 102 }, "slow", null, function () { note.hide(); });
            InfoTip._timer = undefined;
        }, timeout, undefined);
    }

    var left_pos = (($(document).width() - $(note).width()) / 2);
    //alert($(document).width() + "、" + $(note).width() + "、" + left_pos);
    note.css("left", left_pos + "px");
};
//Ajax加载提示类
var ajaxLoading = {};
//开始
ajaxLoading.start = function (message) {
    if ($("#infoLoadingContainer") != null)
        $("#infoLoadingContainer").remove();
    var html = $("<div id='infoLoadingContainer'><div>" +
                    "<span class='text'>" + message + "</span>" +
                    "</div></div>");
    $("body").append(html);
}
//请求完成
ajaxLoading.complete = function () {
    if ($("#infoLoadingContainer") != null)
        $("#infoLoadingContainer").remove();
}

/********************************************/
/*文本框输入时间格式控制                    */
/* 2010-10-19                               */
/********************************************/
//验证粘贴的内容
function regInput(obj, reg, inputStr) {
    var docSel = document.selection.createRange()
    if (docSel.parentElement().tagName != "INPUT") return false
    oSel = docSel.duplicate()
    oSel.text = ""
    var srcRange = obj.createTextRange()
    oSel.setEndPoint("StartToStart", srcRange)
    var str = oSel.text + inputStr + srcRange.text.substr(oSel.text.length)
    return reg.test(str)
}
//格式化时间格式
function verify(text) {
    var hour;
    var minute;
    var tmp;
    var index;
    var textValue = text.value;
    if (textValue.length > 1) { //当长度超过1时，进行处理 
        hour = textValue.substr(0, 2); //取前两位数字，即小时
        if (isNaN(hour)) {//不是数字
            text.value = '00';
            return;
        }
        if (hour < 24) { //10<x<24
            text.value = hour + ':'; //显示小时
            index = textValue.indexOf(':'); //定位冒号
            minute = index > 0 ? textValue.substr(index + 1, 2) : textValue.substr(2, 2);
            if (isNaN(minute)) {//不是数字
                text.value = hour + ':00';
                return;
            }
            if (minute < 59) {
                tmp = hour + ':' + minute;
            } else {
                tmp = hour + ':59';
            }
        } else { //x>=24
            hour = '0' + textValue.substr(0, 1);
            text.value = hour + ':' + text.value.substr(1, 1);
            index = textValue.indexOf(':');
            minute = index > 0 ? textValue.substr(index + 1, 2) : textValue.substr(1, 2);
            if (isNaN(minute)) {//不是数字
                text.value = hour + ':00';
                return;
            }
            if (minute < 59) {
                tmp = hour + ':' + minute;
            } else {
                tmp = hour + ':59';
            }
        }
        text.value = tmp; //输入“小时：分钟”格式
    }
}
//控制键盘的按键，只能输入数字
function inputNumber(e) {
    var keynum;
    var keychar;
    var numcheck;
    if (window.event) // IE
    {
        keynum = e.keyCode
    } else if (e.which) // Netscape/Firefox/Opera
    {
        keynum = e.which
    }
    keychar = String.fromCharCode(keynum);
    return !isNaN(keychar);
}
//验证是否为时间格式
function isTime(obj) {

    if (!obj.value.match(/^([0-9][0-9]):([0-9][0-9])$/)) {
        obj.value = "";
    }
}

//动态加载列表锁定列
function FixList(pColNum) {
    var _colNum = pColNum;     //左边起固定前4列
    var _autowidth = true;
    $('#myFault').each(function () {
        $(this).fixColumn(_colNum, _autowidth);
    });
}

// 时间比较的方法，如果d1时间比d2时间大，则返回true   
function compareDate(d1, d2) {
    return Date.parse(d1.replace(/-/g, "/")) > Date.parse(d2.replace(/-/g, "/"))
}
/********************************************/
/*只能输入数字                              */
/* 2010-10-19                               */
/********************************************/
//只能输入数字
function NumControl() {
    if (event.keyCode < 45 || event.keyCode > 57) {
        event.keyCode = 0;
    }
}


/* -------------------------------------------- */
/* 2010-03-31									*/
/* spectorye									*/
/* 实现弹出模态窗口								*/
/* 需要jquery支持								*/
/* -------------------------------------------- */
function GetCover() {
    var divCover = $("#norepeat_cover");
    if (divCover == null || divCover.length <= 0) {
        divCover = $('<div id="norepeat_cover"></div>');
        divCover.appendTo("body");
    }
    return divCover;
}
function GetModel() {
    var divModel = $("#norepeat_model");
    if (divModel == null || divModel.length <= 0) {
        divModel = $('<div id="norepeat_model"><div class="modelheader">'
			+ '<div class="title"></div>'
			+ '<div class="link">'
			+ '<a href="javascript:showModel();">关闭</a>'
			+ '</div>'
			+ '</div><iframe frameborder="0" scrolling="no">&nbsp;</iframe></div>');
        divModel.appendTo("body");
    }
    return divModel;
}
function window_resize() {
    var divCover = GetCover();
    if (divCover == null || divCover.length <= 0) {
        return;
    }
    var h = Math.max($(window).height(), $(document).height());
    var w = Math.max($(window).width(), $(document).width());
    divCover.width(w - 2);
    divCover.height(h - 2);
    window_scroll();
}
function window_scroll() {
    var divCover = GetCover();
    var divModel = GetModel("");
    if (divCover != null && divModel != null) {
        var scrollTop = $(window).scrollTop();
        var scrollLeft = $(window).scrollLeft();
        //设置浮动层
        divModel.css("top", ($(window).height() - divModel.height()) / 2 + scrollTop);
        divModel.css("left", ($(window).width() - divModel.width()) / 2 + scrollLeft);
    }
}

function showModel(url, title, w, h) {
    if (url && url.length > 0) {
        var nr_cover = GetCover();
        var nr_model = GetModel();
        if (h && h > 0) {
            nr_model.height(h);
        }
        if (w && w > 0) {
            nr_model.width(w);
        }
        if (title && title.length > 0) {
            nr_model.find(".title").text(title);
        }
        var iframe = nr_model.children("iframe");
        if (iframe.length > 0) {
            iframe.attr("src", url);
            iframe.height(h - 28);
            iframe.width(w - 8);
        }
        window_resize();
        nr_cover.show();
        nr_model.show();
        $(window).resize(window_resize);
        $(window).scroll(window_scroll);
    }
    else {
        var nr_cover = GetCover();
        var nr_model = GetModel();
        nr_cover.hide();
        nr_model.hide();
        //nr_cover.remove();
        //nr_model.remove();
        $(window).die("resize", window_resize);
        $(window).die("scroll", window_scroll);
    }
}



/******************************************************
消息处理
******************************************************/
var currentObj = null;
///接受
function confirmDelegate(type, obj) {
    if (messageData[type]) {
        currentObj = obj;
        $.ajax(
            {
                url: messageHandlerConfirmUrl,
                success: delegateResponse,
                type: "POST",
                dataType: "json",
                //data: { "data": $.toJSON(messageData[type]), "messageType": type },
                data: { data: Sys.Serialization.JavaScriptSerializer.serialize(messageData[type]), messageType: type },
                error: AJAX_ERROR
            }
        );
    }
}
//拒绝
function cancelDelegate(type, obj) {
    if (messageData[type]) {
        currentObj = obj;
        $.ajax(
            {
                url: messageHandlerCancelUrl,
                success: delegateResponse,
                type: "POST",
                dataType: "json",
                data: { data: Sys.Serialization.JavaScriptSerializer.serialize(messageData[type]), messageType: type },
                error: AJAX_ERROR
            }
        );
    }
}
//回调
function delegateResponse(res) {
    if (res.Message && res.Result == false) {
        alert(res.Message);
    }
    if (res.Result) {
        messageTotal = messageTotal - 1;
        if (messageTotal == 0) {
            showModel();
            window.location = window.location;
        }
        else {
            if (!currentObj)
                return;
            currentObj.parent().parent().hide();
        }
    }
}
function reLocationInfoMessage() {
    if ($("#norepeat_model")) {
        var top = $(window).height() - $("#norepeat_model").height() - 2;
        $("#norepeat_model").css("top", top);
    }
}
/******************************************************
常量定义
******************************************************/
var AJAX_REQUEST_ERROR = "操作失败，请联系系统管理员！";
var AJAX_REQUEST_UNDEFINED_ERROR = "未定义错误，请联系系统管理员！";

//Ajax错误提示
function AJAX_ERROR(xhr) {
    alert('出现错误，服务器返回错误信息： ' + xhr.statusText);
}