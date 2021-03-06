﻿var toggler = document.getElementsByClassName("caret");
var i;

$(document).ready(function () {
    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }

    $('.carousel.carousel-multi-item.v-2 .carousel-item').each(function () {
        var next = $(this).next();
        if (!next.length) {
            next = $(this).siblings(':first');
        }
        next.children(':first-child').children(':first-child').clone().appendTo($(this).children(':first-child'));

        for (var i = 0; i < $('.carousel.carousel-multi-item.v-2 .carousel-item').length-2; i++) {
            next = next.next();
            if (!next.length) {
                next = $(this).siblings(':first');
            }
            next.children(':first-child').children(':first-child').clone().appendTo($(this).children(':first-child'));
        }
    });
});