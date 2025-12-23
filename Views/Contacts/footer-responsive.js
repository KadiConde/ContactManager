// scripts/footer-responsive.js
document.addEventListener('DOMContentLoaded', function () {
    // Fonction pour calculer et ajuster les hauteurs
    function adjustFooterHeight() {
        const navbar = document.querySelector('.navbar');
        const footer = document.querySelector('.site-footer');
        const main = document.querySelector('main');

        if (navbar && footer && main) {
            // Calculer les hauteurs
            const navbarHeight = navbar.offsetHeight;
            const footerHeight = footer.offsetHeight;

            // Mettre à jour les variables CSS
            document.documentElement.style.setProperty('--navbar-height', `${navbarHeight}px`);
            document.documentElement.style.setProperty('--footer-height', `${footerHeight}px`);

            // Ajuster le padding du main pour la navbar
            main.style.paddingTop = `${navbarHeight}px`;

            // Ajuster la marge du body pour le footer
            document.body.style.marginBottom = `${footerHeight}px`;
        }
    }

    // Fonction pour observer les changements de taille
    function observeResize() {
        const resizeObserver = new ResizeObserver(() => {
            adjustFooterHeight();
        });

        // Observer la navbar et le footer
        const navbar = document.querySelector('.navbar');
        const footer = document.querySelector('.site-footer');

        if (navbar) resizeObserver.observe(navbar);
        if (footer) resizeObserver.observe(footer);

        // Observer les changements de contenu dans le footer
        const footerContent = footer?.querySelector('.footer-content-grid');
        if (footerContent) resizeObserver.observe(footerContent);
    }

    // Fonction pour gérer le responsive mobile
    function handleMobileResponsive() {
        const isMobile = window.innerWidth <= 768;
        const footer = document.querySelector('.site-footer');

        if (footer) {
            if (isMobile) {
                // Mode mobile
                footer.classList.add('mobile-footer');
                document.documentElement.style.setProperty('--footer-height', 'auto');
            } else {
                // Mode desktop
                footer.classList.remove('mobile-footer');
            }
        }
    }

    // Fonction pour gérer les fenêtres de taille réduite
    function handleWindowResize() {
        adjustFooterHeight();
        handleMobileResponsive();
    }

    // Initialiser
    adjustFooterHeight();
    observeResize();
    handleMobileResponsive();

    // Écouter les événements de redimensionnement
    window.addEventListener('resize', handleWindowResize);
    window.addEventListener('orientationchange', handleWindowResize);

    // Recalculer après le chargement des images
    window.addEventListener('load', adjustFooterHeight);

    // Recalculer après les animations CSS
    document.addEventListener('animationend', adjustFooterHeight);
    document.addEventListener('transitionend', adjustFooterHeight);

    // API pour recalculer manuellement si besoin
    window.recalculateLayout = adjustFooterHeight;
});

// Alternative si ResizeObserver n'est pas supporté
if (!window.ResizeObserver) {
    console.warn('ResizeObserver non supporté, utilisation de fallback');

    let resizeTimeout;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(function () {
            if (window.recalculateLayout) {
                window.recalculateLayout();
            }
        }, 250);
    });
}