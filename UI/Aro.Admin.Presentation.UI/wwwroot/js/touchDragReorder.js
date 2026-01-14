// Touch and mouse-based drag and drop reordering
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
    isDragging: false,
    boundHandlers: [],

    init: function (containerId, dotNetReference) {
        this.dotNetRef = dotNetReference;
        this.container = document.getElementById(containerId);

        if (!this.container) {
            console.error('Touch drag container not found:', containerId);
            return;
        }

        // Clean up previous handlers first
        this.dispose();
        this.container = document.getElementById(containerId);
        this.dotNetRef = dotNetReference;

        this.attachHandlers();
    },

    attachHandlers: function () {
        const handles = this.container.querySelectorAll('.touch-drag-handle');
        this.boundHandlers = [];

        handles.forEach((handle, index) => {
            // Touch events for mobile
            const touchStartHandler = (e) => this.handleStart(e, index, true);
            const touchMoveHandler = (e) => this.handleMove(e, true);
            const touchEndHandler = (e) => this.handleEnd(e, true);

            // Mouse events for desktop
            const mouseDownHandler = (e) => this.handleStart(e, index, false);

            handle.addEventListener('touchstart', touchStartHandler, { passive: false });
            handle.addEventListener('touchmove', touchMoveHandler, { passive: false });
            handle.addEventListener('touchend', touchEndHandler, { passive: false });
            handle.addEventListener('mousedown', mouseDownHandler, { passive: false });

            this.boundHandlers.push({
                element: handle,
                touchstart: touchStartHandler,
                touchmove: touchMoveHandler,
                touchend: touchEndHandler,
                mousedown: mouseDownHandler
            });
        });

        // Global mouse move and up handlers for desktop drag
        this.globalMouseMove = (e) => this.handleMove(e, false);
        this.globalMouseUp = (e) => this.handleEnd(e, false);

        document.addEventListener('mousemove', this.globalMouseMove);
        document.addEventListener('mouseup', this.globalMouseUp);
    },

    handleStart: function (e, index, isTouch) {
        // For mouse, only proceed on left click
        if (!isTouch && e.button !== 0) return;

        e.preventDefault();
        e.stopPropagation();

        const clientY = isTouch ? e.touches[0].clientY : e.clientY;
        this.startY = clientY;
        this.currentY = clientY;
        this.draggedIndex = index;
        this.isDragging = true;

        // Get the parent row element
        const rows = this.container.querySelectorAll('.selection-row');
        this.items = Array.from(rows);
        this.draggedElement = this.items[index];

        if (!this.draggedElement) return;

        // Calculate item height for reordering calculation
        const rect = this.draggedElement.getBoundingClientRect();
        const nextSibling = this.items[index + 1];
        if (nextSibling) {
            const nextRect = nextSibling.getBoundingClientRect();
            this.itemHeight = nextRect.top - rect.top;
        } else {
            this.itemHeight = rect.height + 10;
        }

        // Add dragging class
        this.draggedElement.classList.add('touch-dragging');
        document.body.style.cursor = 'grabbing';
        document.body.style.userSelect = 'none';

        // Store original positions
        this.items.forEach((item, i) => {
            const itemRect = item.getBoundingClientRect();
            item.dataset.originalTop = itemRect.top;
        });
    },

    handleMove: function (e, isTouch) {
        if (!this.isDragging || this.draggedElement === null) return;

        e.preventDefault();

        const clientY = isTouch ? e.touches[0].clientY : e.clientY;
        this.currentY = clientY;
        const deltaY = this.currentY - this.startY;

        // Move the dragged element visually
        this.draggedElement.style.transform = `translateY(${deltaY}px)`;
        this.draggedElement.style.zIndex = '1000';
        this.draggedElement.style.position = 'relative';

        // Calculate which position we're over
        const moveSteps = Math.round(deltaY / this.itemHeight);

        // Update visual indicators on other items
        this.items.forEach((item, i) => {
            if (i === this.draggedIndex) return;

            const targetIndex = this.draggedIndex + moveSteps;

            if (moveSteps > 0 && i > this.draggedIndex && i <= targetIndex) {
                // Items that should move up
                item.style.transform = `translateY(-${this.itemHeight}px)`;
                item.style.transition = 'transform 0.15s ease';
            } else if (moveSteps < 0 && i < this.draggedIndex && i >= targetIndex) {
                // Items that should move down
                item.style.transform = `translateY(${this.itemHeight}px)`;
                item.style.transition = 'transform 0.15s ease';
            } else {
                item.style.transform = '';
                item.style.transition = 'transform 0.15s ease';
            }
        });
    },

    handleEnd: function (e, isTouch) {
        if (!this.isDragging || this.draggedElement === null) return;

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
        document.body.style.cursor = '';
        document.body.style.userSelect = '';

        // Notify Blazor of the reorder if position changed
        if (newIndex !== this.draggedIndex && this.dotNetRef) {
            this.dotNetRef.invokeMethodAsync('OnTouchReorder', this.draggedIndex, newIndex);
        }

        // Reset state
        this.draggedElement = null;
        this.draggedIndex = null;
        this.startY = 0;
        this.currentY = 0;
        this.isDragging = false;
    },

    dispose: function () {
        // Remove element-specific handlers
        if (this.boundHandlers) {
            this.boundHandlers.forEach(handler => {
                if (handler.element) {
                    handler.element.removeEventListener('touchstart', handler.touchstart);
                    handler.element.removeEventListener('touchmove', handler.touchmove);
                    handler.element.removeEventListener('touchend', handler.touchend);
                    handler.element.removeEventListener('mousedown', handler.mousedown);
                }
            });
        }
        this.boundHandlers = [];

        // Remove global handlers
        if (this.globalMouseMove) {
            document.removeEventListener('mousemove', this.globalMouseMove);
        }
        if (this.globalMouseUp) {
            document.removeEventListener('mouseup', this.globalMouseUp);
        }

        this.dotNetRef = null;
        this.container = null;
        this.isDragging = false;
    }
};
