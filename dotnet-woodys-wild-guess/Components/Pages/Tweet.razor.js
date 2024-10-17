/**
 * Adds support for Tweets.
 * @param {*} dotnetHelper 
 * @returns 
 */
export const addTweetSupport = async (dotnetHelper) => {
    if (typeof dotnetHelper === 'undefined') {
        console.error('dotnetHelper is undefined.');
        return;
    }

    // Get the toast element by ID
    const successToastElement = document.getElementById('successToast');
    const successToast = new bootstrap.Toast(successToastElement);

    $('#send-tweet-btn').click(async () => {
        console.debug('send-tweet-btn clicked');
        const tweet = $('#tweet-text').val();
        if (tweet.length > 280) {
            showAlert('Tweet is too long: Max 280 characters.', 'warning');
            return;
        }

        const response = await dotnetHelper.invokeMethodAsync('PostTweet', tweet);
        if (response === 'success') {
            $('#tweet-text').val('');
            successToast.show();
        } else if (response === 'unauthorized') {
            $('#tweet-text').val('');
            showAlert('You are not authorized to post tweets.', 'danger');
        }
        else {
            showAlert('Failure to post tweet.', 'danger');
        }
    });
}

// Function to show a Bootstrap alert at the top of a specific container
function showAlert(message, type = 'primary', duration = 5000, containerSelector = '.body-container') {
    // Create the alert element
    const alertElement = document.createElement('div');
    alertElement.className = `alert alert-${type} alert-dismissible fade show mt-1`;
    alertElement.role = 'alert';
    alertElement.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    // Find the container where the alert should be placed
    const container = document.querySelector(containerSelector);

    // If the container exists, insert the alert at the top
    if (container) {
        container.prepend(alertElement);

        // Automatically remove the alert after the specified duration
        setTimeout(() => {
            alertElement.classList.remove('show');
            alertElement.classList.add('hide');
            alertElement.addEventListener('transitionend', () => alertElement.remove());
        }, duration);
    } else {
        console.error(`Container with selector "${containerSelector}" not found.`);
    }
}