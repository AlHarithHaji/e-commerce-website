const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

$(document).ready(function () {
    // search bar page
    $("#search-button").click(function () {
        $(".search-bar-wrap").show();
    })
    $(".search-close").click(function () {
        $(".search-bar-wrap").hide();
    })

    $(".size-color-selector .sizes").click(function () {
        $(this).siblings("").removeClass("selected")
        $(this).addClass("selected")
    })
})