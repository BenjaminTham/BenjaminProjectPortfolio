// Hide Navbar on scroll down
/*var prevScrollpos = window.pageYOffset;
window.onscroll = function() {
var currentScrollPos = window.pageYOffset;
  if (prevScrollpos > currentScrollPos) {
    document.getElementById("navbar_main").style.top = "0";
  } else {
    document.getElementById("navbar_main").style.top = "-80px"; /!* width horizontal navbar *!/
  }
  prevScrollpos = currentScrollPos;

}*/

const mainNavigation = document.querySelector(".main-navigation");
const overlay = mainNavigation.querySelector(".overlay");
const toggler = mainNavigation.querySelector(".navbar-toggler");

const openSideNav = () => mainNavigation.classList.add("active");
const closeSideNav = () => mainNavigation.classList.remove("active");
toggler.addEventListener("click", openSideNav);
overlay.addEventListener("click", closeSideNav); 

// Scroll to top
/*
document.addEventListener("DOMContentLoaded", function () {
    window.addEventListener('scroll', function () {
        if (window.scrollY > 150) {
            //document.getElementById('navbar_main').classList.add('fixed-top');
            document.getElementById('navbar_main').classList.add('nav-menu-sticky');
            // add padding top to show content behind navbar
            navbar_height = document.querySelector('.navbar').offsetHeight;
            document.body.style.paddingTop = navbar_height + 'px';
        } else {
            //document.getElementById('navbar_main').classList.remove('fixed-top');
            document.getElementById('navbar_main').classList.remove('nav-menu-sticky');
            // remove padding top from body
            document.body.style.paddingTop = '0';
        }
    });
});*/
