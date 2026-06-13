export const vClickOutside = {
    mounted(el, binding) {
        el.__clickOutsideHandler = (e) => {
            if (!el.contains(e.target)) binding.value();
        };
        document.addEventListener('click', el.__clickOutsideHandler);
    },
    unmounted(el) {
        document.removeEventListener('click', el.__clickOutsideHandler);
    }
};