function submitSearchForm(current) {
    const form = document.getElementById('from-search');
    const currentInput = document.createElement('input');
    currentInput.type = 'hidden';
    currentInput.name = 'current';
    currentInput.value = current;

    form.appendChild(currentInput);
    form.submit();
}



