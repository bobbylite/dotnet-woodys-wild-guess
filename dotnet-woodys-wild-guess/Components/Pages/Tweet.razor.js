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

    $('#send-tweet-btn').click(async () => {
        console.debug('send-tweet-btn clicked');
        const tweet = $('#tweet-text').val();
        if (tweet.length > 280) {
            alert('Tweet is too long. Max 280 characters.');
            return;
        }

        const response = await dotnetHelper.invokeMethodAsync('PostTweet', tweet);
        if (response === 'success') {
            $('#tweet-text').val('');
            alert('Tweet sent.');
        } else {
            alert('Failed to send tweet.');
        }
    });
}