function toggleNavMobile(elem) {
    const div = document.querySelector('#navbar-links');
    if (div != null) {
        if (div.classList.contains("hidden")) {
            div.classList.remove("hidden");
        } else {
            div.classList.add("hidden");
            if (document.activeElement == elem) {
                elem.blur();
            }
        }
    }
}

function sizeNav() {
    if (document.getElementById("special-announcement") != null && screen.width < 767) {
        var top1 = document.getElementById("special-announcement").offsetHeight;
        document.getElementById("navbar-links").style.top = String(62 + top1) + "px";
    } else if (screen.width >= 767) {
        document.getElementById("navbar-links").style.top = String(0) + "px";
    }
}

function findElementsWithStyles() {
    var allElements = document.getElementsByTagName('*');
    var matchedElements = [];
  
    for (var i = 0; i < allElements.length; i++) {
      var element = allElements[i];
      var elementStyles = window.getComputedStyle(element);
  
      // Check the computed styles for width and margin properties
      var width = elementStyles.getPropertyValue('width');
      var marginLeft = elementStyles.getPropertyValue('margin-left');
      var marginRight = elementStyles.getPropertyValue('margin-right');
  
      // Check if the styles match your criteria
      if (width === '100%' && (marginLeft || marginRight)) {
        matchedElements.push(element);
      }
    }
  
    return matchedElements;
  }

// prevent elements with following traits from shifting to the right because of the margin
function adjustWidthWithMargin() {
    var elements = document.querySelectorAll('[style*="width: 100%; margin"], [style*="width: 100%; margin-left"], [style*="width: 100%; margin-right"], [class^="m-"].w-full, [class^="mx-"].w-full, [class^="mr-"].w-full, [class^="ml-"].w-full');
    for (var i = 0; i < elements.length; i++) {
        console.log("element");
        var element = elements[i];
        var marginValueL = element.style.marginLeft;
        console.log("ml: " + marginValueL);
        var marginValueR = element.style.marginRight;
        element.style.width = 'calc(100% - ' + marginValueL + + ' - ' + marginValueR + ')';
    }
}
function adjustHeightWithMargin() {
    var elements = document.querySelectorAll('[style*="height: 100%; margin"], [style*="height: 100%; margin-top"], [style*="width: 100%; margin-bottom"], [class^="m-"].h-full, [class^="my-"].h-full, [class^="mt-"].h-full, [class^="mb-"].h-full');
    for (var i = 0; i < elements.length; i++) {
        console.log("element");
        var element = elements[i];
        var marginValueT = element.style.marginTop;
        console.log("mt: " + marginValueT);
        var marginValueB = element.style.marginBottom;
        element.style.height = 'calc(100% - ' + marginValueT + + ' - ' + marginValueB + ')';
    }
}

function BlazorScrollToId(id) {
    const element = document.getElementById(id);
    if (element instanceof HTMLElement) {
        element.scrollIntoView({
            behavior: "smooth",
            block: "start",
            inline: "nearest"
        });
    }
}

window.onbeforeunload = function (e) {
    localStorage.setItem('scrollpos', window.scrollY);
    localStorage.setItem('fromPage', window.location.href);
};

window.addEventListener("resize", function (event) {
    sizeNav()
})

window.addEventListener('load', function() {
    adjustWidthWithMargin();
    adjustHeightWithMargin();
  });


sizeNav();