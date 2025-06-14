// Enhanced contact form validation and UX improvements
document.addEventListener('DOMContentLoaded', function () {
    const contactForm = document.getElementById('contactForm');
    const submitButton = document.getElementById('submitButton');

    if (!contactForm) return;

    // Original button text
    const originalButtonText = submitButton.innerHTML;

    // Add Bootstrap validation classes
    contactForm.addEventListener('submit', function (event) {
        if (!contactForm.checkValidity()) {
            event.preventDefault();
            event.stopPropagation();
        } else {
            // Show loading state
            submitButton.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Sending...';
            submitButton.disabled = true;
        }

        contactForm.classList.add('was-validated');
    });

    // Real-time validation feedback
    const inputs = contactForm.querySelectorAll('input, select, textarea');
    inputs.forEach(input => {
        input.addEventListener('blur', function () {
            validateField(this);
        });

        input.addEventListener('input', function () {
            if (this.classList.contains('is-invalid')) {
                validateField(this);
            }
        });
    });

    function validateField(field) {
        const isValid = field.checkValidity();

        if (isValid) {
            field.classList.remove('is-invalid');
            field.classList.add('is-valid');
        } else {
            field.classList.remove('is-valid');
            field.classList.add('is-invalid');
        }
    }

    // Phone number formatting
    const phoneInput = document.getElementById('phone');
    if (phoneInput) {
        phoneInput.addEventListener('input', function (e) {
            let value = e.target.value.replace(/\D/g, '');

            // Format international numbers
            if (value.startsWith('359')) {
                value = '+359 ' + value.substring(3, 6) + ' ' + value.substring(6, 9) + ' ' + value.substring(9, 12);
            } else if (value.startsWith('0')) {
                value = value.substring(0, 4) + ' ' + value.substring(4, 7) + ' ' + value.substring(7, 10);
            }

            e.target.value = value.trim();
        });
    }

    // Character counter for message textarea
    const messageTextarea = document.getElementById('message');
    if (messageTextarea) {
        const maxLength = 2000;
        const counterDiv = document.createElement('div');
        counterDiv.className = 'form-text text-muted small mt-1';
        messageTextarea.parentNode.appendChild(counterDiv);

        function updateCounter() {
            const currentLength = messageTextarea.value.length;
            const remaining = maxLength - currentLength;
            counterDiv.textContent = `${currentLength}/${maxLength} characters`;

            if (remaining < 100) {
                counterDiv.className = 'form-text text-warning small mt-1';
            } else if (remaining < 0) {
                counterDiv.className = 'form-text text-danger small mt-1';
            } else {
                counterDiv.className = 'form-text text-muted small mt-1';
            }
        }

        messageTextarea.addEventListener('input', updateCounter);
        updateCounter(); // Initial count
    }

    // Auto-hide success/error messages after 5 seconds
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(alert => {
        setTimeout(() => {
            alert.style.transition = 'opacity 0.5s';
            alert.style.opacity = '0';
            setTimeout(() => {
                alert.remove();
            }, 500);
        }, 5000);
    });

    // Project type specific behavior
    const projectTypeSelect = document.getElementById('projectType');
    const budgetSelect = document.getElementById('budget');

    if (projectTypeSelect && budgetSelect) {
        projectTypeSelect.addEventListener('change', function () {
            const selectedType = this.value;

            // Update budget options based on project type
            if (selectedType === 'Consultation') {
                budgetSelect.innerHTML = `
                    <option value="">Select budget range...</option>
                    <option value="€100 - €500">€100 - €500</option>
                    <option value="€500 - €1,000">€500 - €1,000</option>
                    <option value="€1,000+">€1,000+</option>
                    <option value="Let's discuss">Let's discuss</option>
                `;
            } else {
                budgetSelect.innerHTML = `
                    <option value="">Select budget range...</option>
                    <option value="Under €1,000">Under €1,000</option>
                    <option value="€1,000 - €2,500">€1,000 - €2,500</option>
                    <option value="€2,500 - €5,000">€2,500 - €5,000</option>
                    <option value="€5,000+">€5,000+</option>
                    <option value="Let's discuss">Let's discuss</option>
                `;
            }
        });
    }

    // Form reset after successful submission (if redirected back)
    if (window.location.search.includes('success=true')) {
        contactForm.reset();
        contactForm.classList.remove('was-validated');
        inputs.forEach(input => {
            input.classList.remove('is-valid', 'is-invalid');
        });
    }

    // Restore button state if form submission failed
    if (submitButton.disabled) {
        submitButton.disabled = false;
        submitButton.innerHTML = originalButtonText;
    }
});