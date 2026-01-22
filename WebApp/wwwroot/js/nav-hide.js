(() => {
  const HIDE_CLASS = "nav-hidden";
  const THRESHOLD = 12;     // hvor meget man skal scroll før vi reagerer
  const TOP_RESET = 5;      // hvis man er tæt på toppen -> vis nav

  function attachScrollHandler(getScrollTop) {
    let last = getScrollTop();

    return () => {
      // Hvis mobil-menuen er åben, skal nav ikke skjules (så UX ikke føles broken)
      const mobileMenuOpen = document.querySelector(".mobile-menu.open");
      if (mobileMenuOpen) {
        document.body.classList.remove(HIDE_CLASS);
        last = getScrollTop();
        return;
      }

      const current = getScrollTop();

      // tæt på top -> vis nav
      if (current <= TOP_RESET) {
        document.body.classList.remove(HIDE_CLASS);
        last = current;
        return;
      }

      const delta = current - last;

      // ignorer mikrobevægelser
      if (Math.abs(delta) < THRESHOLD) return;

      if (delta > 0) {
        // scroller ned -> skjul
        document.body.classList.add(HIDE_CLASS);
      } else {
        // scroller op -> vis
        document.body.classList.remove(HIDE_CLASS);
      }

      last = current;
    };
  }

  function init() {
    // 1) Window scroll (alle normale pages)
    const onWindowScroll = attachScrollHandler(() =>
      window.pageYOffset || document.documentElement.scrollTop || 0
    );
    window.addEventListener("scroll", onWindowScroll, { passive: true });

    // 2) About page scroll container (din scroll-snap side)
    const about = document.querySelector(".about-scroll");
    if (about) {
      const onAboutScroll = attachScrollHandler(() => about.scrollTop || 0);
      about.addEventListener("scroll", onAboutScroll, { passive: true });
    }
  }

  // Vent til DOM er klar
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", init);
  } else {
    init();
  }
})();
