// Country dropdown scroll functionality
window.scrollToSelectedCountry = function (selectedCountry) {
    // Find the MudAutocomplete popover
    const popover = document.querySelector('.mud-popover-open .mud-list');
    if (!popover) return;

    // Find all list items
    const items = popover.querySelectorAll('.mud-list-item');

    for (let i = 0; i < items.length; i++) {
        const item = items[i];
        const text = item.textContent.trim();

        if (text === selectedCountry) {
            // Scroll the item into view
            item.scrollIntoView({ block: 'center', behavior: 'auto' });

            // Remove any existing selection highlight
            items.forEach(el => el.classList.remove('country-selected-item'));

            // Add highlight class to simulate selection
            item.classList.add('country-selected-item');

            break;
        }
    }
};
