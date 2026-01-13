// Touch-based drag and drop reordering for mobile devices
window.touchDragReorder = {
    dotNetRef: null,
    container: null,
    draggedElement: null,
    draggedIndex: null,
    placeholder: null,
    items: [],
    startY: 0,
    currentY: 0,
    itemHeight: 0,

    init: function (containerId, dotNetReference) {
        this.dotNetRef = dotNetReference;
        this.container = document.getElementById(containerId);

        if (!this.container) {
            console.error('Touch drag container not found:', containerId);
            return;
        }

        this.attachTouchHandlers();
    },

    attachTouchHandlers: function () {
        const handles = this.container.querySelectorAll('.touch-drag-handle');

        handles.forEach((handle, index) => {
            handle.addEventListener('touchstart', (e) => this.handleTouchStart(e, index), { passive: false });
            handle.addEventListener('touchmove', (e) => this.handleTouchMove(e), { passive: false });
            handle.addEventListener('touchend', (e) => this.handleTouchEnd(e), { passive: false });
        });
    },

    handleTouchStart: function (e, index) {
        e.preventDefault();

        const touch = e.touches[0];
        this.startY = touch.clientY;
        this.currentY = touch.clientY;
        this.draggedIndex = index;

        // Get the parent row element
        const rows = this.container.querySelectorAll('.selection-row');
        this.items = Array.from(rows);
        this.draggedElement = this.items[index];

        if (!this.draggedElement) return;

        // Calculate item height for reordering calculation
        this.itemHeight = this.draggedElement.offsetHeight + 10; // include gap

        // Add dragging class
        this.draggedElement.classList.add('touch-dragging');

        // Store original positions
        this.items.forEach((item, i) => {
            const rect = item.getBoundingClientRect();
            item.dataset.originalTop = rect.top;
        });
    },

    handleTouchMove: function (e) {
        if (this.draggedElement === null) return;

        e.preventDefault();

        const touch = e.touches[0];
        this.currentY = touch.clientY;
        const deltaY = this.currentY - this.startY;

        // Move the dragged element visually
        this.draggedElement.style.transform = `translateY(${deltaY}px)`;
        this.draggedElement.style.zIndex = '1000';
        this.draggedElement.style.position = 'relative';

        // Calculate which position we're over
        const moveThreshold = this.itemHeight / 2;
        const moveSteps = Math.round(deltaY / this.itemHeight);

        // Update visual indicators on other items
        this.items.forEach((item, i) => {
            if (i === this.draggedIndex) return;

            const targetIndex = this.draggedIndex + moveSteps;

            if (moveSteps > 0 && i > this.draggedIndex && i <= targetIndex) {
                // Items that should move up
                item.style.transform = `translateY(-${this.itemHeight}px)`;
                item.style.transition = 'transform 0.2s ease';
            } else if (moveSteps < 0 && i < this.draggedIndex && i >= targetIndex) {
                // Items that should move down
                item.style.transform = `translateY(${this.itemHeight}px)`;
                item.style.transition = 'transform 0.2s ease';
            } else {
                item.style.transform = '';
                item.style.transition = 'transform 0.2s ease';
            }
        });
    },

    handleTouchEnd: function (e) {
        if (this.draggedElement === null) return;

        e.preventDefault();

        const deltaY = this.currentY - this.startY;
        const moveSteps = Math.round(deltaY / this.itemHeight);
        let newIndex = this.draggedIndex + moveSteps;

        // Clamp to valid range
        newIndex = Math.max(0, Math.min(newIndex, this.items.length - 1));

        // Reset all transforms
        this.items.forEach(item => {
            item.style.transform = '';
            item.style.transition = '';
            item.style.zIndex = '';
            item.style.position = '';
        });

        this.draggedElement.classList.remove('touch-dragging');

        // Notify Blazor of the reorder if position changed
        if (newIndex !== this.draggedIndex && this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnTouchReorder', this.draggedIndex, newIndex);
        }

        // Reset state
        this.draggedElement = null;
        this.draggedIndex = null;
        this.startY = 0;
        this.currentY = 0;
    },

    dispose: function () {
        if (this.container) {
            const handles = this.container.querySelectorAll('.touch-drag-handle');
            handles.forEach(handle => {
                handle.replaceWith(handle.cloneNode(true)); // Remove all event listeners
            });
        }
        this.dotNetRef = null;
        this.container = null;
    }
};
