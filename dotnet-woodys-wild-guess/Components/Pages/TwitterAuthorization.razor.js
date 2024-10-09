/**
 * Adds support for Twitter authorization.
 * @param {*} dotnetHelper 
 * @returns 
 */
export const addTwitterAuthorizationSupport = (dotnetHelper, options) => {
    if (typeof dotnetHelper === 'undefined') {
        console.error('dotnetHelper is undefined.');
        return;
    }

    if (typeof options === 'undefined') {
        console.error('dotnetHelper is undefined.');
        return;
    }

    let deserializedOptions = JSON.parse(options);
    console.debug('Deserialized options:', deserializedOptions);
}