window.enableImageDragReorderAnimated = (containerId) => {
    const container = document.getElementById(containerId);
    let dragSrcEl = null;

    function handleDragStart(e) {
        dragSrcEl = this;
        this.classList.add('dragging');
        e.dataTransfer.effectAllowed = 'move';
        e.dataTransfer.setData('text/plain', this.dataset.id);
    }

    function handleDragEnd(e) {
        this.classList.remove('dragging');
        container.querySelectorAll('[draggable="true"]').forEach(item => item.classList.remove('drag-over'));
    }

    function handleDragOver(e) {
        e.preventDefault();
        e.dataTransfer.dropEffect = 'move';
        if (this !== dragSrcEl) this.classList.add('drag-over');
        return false;
    }

    function handleDragLeave(e) {
        this.classList.remove('drag-over');
    }

    function handleDrop(e) {
        e.stopPropagation();
        this.classList.remove('drag-over');
        dragSrcEl.classList.remove('dragging');

        const draggedId = e.dataTransfer.getData('text/plain');
        const droppedId = this.dataset.id;

        if (draggedId === droppedId) return false;

        const draggedEl = container.querySelector(`[data-id='${draggedId}']`);
        const droppedEl = this;

        // Animate: insert before or after
        const allItems = Array.from(container.children).filter(c => c.dataset.id);
        const draggedIndex = allItems.indexOf(draggedEl);
        const droppedIndex = allItems.indexOf(droppedEl);

        if (draggedIndex < droppedIndex) {
            container.insertBefore(draggedEl, droppedEl.nextSibling);
        } else {
            container.insertBefore(draggedEl, droppedEl);
        }

        // Send new order to Blazor
        const orderedIds = Array.from(container.children)
            .filter(c => c.dataset.id)
            .map(c => c.dataset.id);

        // DotNet.invokeMethodAsync('YourAssemblyName', 'UpdateImageOrder', orderedIds);
        return false;
    }

    const items = container.querySelectorAll('[draggable="true"]');
    items.forEach(item => {
        item.addEventListener('dragstart', handleDragStart, false);
        item.addEventListener('dragend', handleDragEnd, false);
        item.addEventListener('dragover', handleDragOver, false);
        item.addEventListener('dragleave', handleDragLeave, false);
        item.addEventListener('drop', handleDrop, false);
    });
};
