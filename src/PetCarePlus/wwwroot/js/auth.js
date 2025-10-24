function mapFormData(form) {
    const formData = new FormData(form);
    const mapping = form.dataset.apiMapping ? JSON.parse(form.dataset.apiMapping) : {};
    const body = {};
    formData.forEach((value, key) => {
        const targetKey = mapping[key] ?? key;
        body[targetKey] = value;
    });
    return body;
}

document.querySelectorAll('form[data-api="json"]').forEach(form => {
    form.addEventListener('submit', async event => {
        event.preventDefault();
        const body = mapFormData(form);

        const response = await fetch(form.action, {
            method: form.dataset.method ?? 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(body)
        });

        const statusText = form.querySelector('.pc-form-status') ?? document.createElement('p');
        statusText.classList.add('pc-form-status');
        form.appendChild(statusText);

        if (response.ok) {
            const contentType = response.headers.get('Content-Type') ?? '';
            if (contentType.includes('application/json')) {
                const data = await response.json();
                statusText.textContent = data.message ?? 'Операція успішна.';
            } else {
                statusText.textContent = 'Операція успішна.';
            }
            statusText.style.color = 'var(--secondary)';
        } else {
            const error = await response.text();
            statusText.textContent = 'Сталася помилка: ' + error;
            statusText.style.color = '#d43f3f';
        }
    });
});
