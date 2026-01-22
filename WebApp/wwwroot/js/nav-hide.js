(() => {
  const HIDE_CLASS = "nav-hidden";
  const THRESHOLD = 12;
  const TOP_RESET = 5;

  let windowHandlerAttached = false;
  let aboutHandlerAttached = false;

  function attachScrollHandler(getScrollTop) {
    let last = getScrollTop();

    return () => {
      const mobileMenuOpen = document.querySelector(".mobile-menu.open");
      if (mobileMenuOpen) {
        document.body.classList.remove(HIDE_CLASS);
        last = getScrollTop();
        return;
      }

      const current = getScrollTop();

      if (current <= TOP_RESET) {
        document.body.classList.remove(HIDE_CLASS);
        last = current;
        return;
      }

      const delta = current - last;
      if (Math.abs(delta) < THRESHOLD) return;

      if (delta > 0) document.body.classList.add(HIDE_CLASS);
      else document.body.classList.remove(HIDE_CLASS);

      last = current;
    };
  }

  function attachWindowScroll() {
    if (windowHandlerAttached) return;

    const onWindowScroll = attachScrollHandler(() =>
      window.pageYOffset || document.documentElement.scrollTop || 0
    );

    window.addEventListener("scroll", onWindowScroll, { passive: true });
    windowHandlerAttached = true;
  }

  function attachAboutScrollIfPresent() {
    const about = document.querySelector(".about-scroll");
    if (!about) return;

    // Hvis vi allerede har attached, så gør intet
    if (aboutHandlerAttached) return;

    const onAboutScroll = attachScrollHandler(() => about.scrollTop || 0);
    about.addEventListener("scroll", onAboutScroll, { passive: true });
    aboutHandlerAttached = true;
  }

  function init() {
    attachWindowScroll();
    attachAboutScrollIfPresent();
  }

  // 👇 Eksponér en funktion vi kan kalde fra Blazor efter render
  window.navHideInit = () => {
    init();
  };

  // Init ved load
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
