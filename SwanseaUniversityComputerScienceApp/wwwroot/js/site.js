//$(document).ready(function (e) {
//    $("#search").keydown(function (e) {
//        var key = e.keyCode;
//        if (!(key === 8 || key === 32 || (key >= 65 && key <= 90))) {
//            e.preventDefault();
//        }
//    });
//});

// When the user scrolls the page, execute myFunction 
window.onscroll = function () { myFunction() };

function myFunction() {
    var winScroll = document.body.scrollTop || document.documentElement.scrollTop;
    var height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
    var scrolled = (winScroll / height) * 100;
    document.getElementById("myBar").style.width = scrolled + "%";
}