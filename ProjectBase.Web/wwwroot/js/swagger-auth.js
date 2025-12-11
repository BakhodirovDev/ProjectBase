(function () {
    'use strict';

    let currentToken = null;

    function initializeSwaggerAuth() {
        if (window.ui && document.querySelector('.swagger-ui')) {
            interceptSignInRequests();
            interceptFetchRequests();
        } else {
            setTimeout(initializeSwaggerAuth, 500);
        }
    }

    function interceptSignInRequests() {
        const originalFetch = window.fetch;

        window.fetch = function (...args) {
            const [url, options] = args;

            if (url.includes('/api/v1/Auth/SignIn') && options?.method === 'POST') {
                return originalFetch.apply(this, args)
                    .then(response => {
                        if (response.ok) {
                            const clonedResponse = response.clone();
                            clonedResponse.json().then(data => {
                                if (data.isSuccess && data.value && data.value.accessToken) {
                                    const token = data.value.accessToken;
                                    currentToken = token;
                                    setTimeout(() => {
                                        setAuthorizationSilently(token);
                                    }, 100);
                                }
                            }).catch(() => { });
                        }
                        return response;
                    })
                    .catch(error => { throw error; });
            }

            return originalFetch.apply(this, args);
        };
    }

    function setAuthorizationSilently(token) {
        try {
            if (window.ui && window.ui.authActions) {
                const authSchemes = [
                    'Bearer (apiKey)',
                    'Bearer',
                    'apiKey',
                    'Authorization'
                ];

                const authData = {};
                authSchemes.forEach(schemeName => {
                    authData[schemeName] = {
                        name: schemeName,
                        schema: {
                            type: 'apiKey',
                            in: 'header',
                            name: 'Authorization'
                        },
                        value: `Bearer ${token}`
                    };
                });

                window.ui.authActions.authorize(authData);
                setTimeout(() => {
                    updateUIState();
                }, 50);
                return;
            }
        } catch (error) { }

        setAuthorizationViaDOM(token);
    }

    function updateUIState() {
        const authorizeBtn = document.querySelector('.btn.authorize');
        if (authorizeBtn) {
            const lockIcon = authorizeBtn.querySelector('.unlocked');
            if (lockIcon) {
                lockIcon.classList.remove('unlocked');
                lockIcon.classList.add('locked');
            }
        }

        const lockIcons = document.querySelectorAll('.authorization__btn');
        lockIcons.forEach(icon => {
            icon.classList.remove('authorization__btn_locked');
            icon.classList.add('authorization__btn_unlocked');
        });
    }

    function setAuthorizationViaDOM(token) {
        if (window.ui && window.ui.getState) {
            const state = window.ui.getState();
            const spec = state.spec;

            if (spec && spec.json && spec.json.components && spec.json.components.securitySchemes) {
                const securitySchemes = spec.json.components.securitySchemes;
                Object.keys(securitySchemes).forEach(schemeName => {
                    if (window.ui.authActions) {
                        const authData = {};
                        authData[schemeName] = {
                            name: schemeName,
                            schema: securitySchemes[schemeName],
                            value: `Bearer ${token}`
                        };
                        window.ui.authActions.authorize(authData);
                    }
                });
            }
        }
    }

    function interceptFetchRequests() {
        const originalFetch = window.fetch;

        window.fetch = function (...args) {
            const [url, options] = args;

            if (url.includes('/api/') && currentToken && options) {
                options.headers = options.headers || {};
                if (!options.headers.Authorization && !options.headers.authorization) {
                    options.headers.Authorization = `Bearer ${currentToken}`;
                }
            }

            return originalFetch.apply(this, args);
        };
    }

    function preventModalOpen() {
        document.addEventListener('click', function (event) {
            const target = event.target;
            if (target.closest('.btn.authorize') && currentToken) {
                event.preventDefault();
                event.stopPropagation();
                return false;
            }
        }, true);
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            initializeSwaggerAuth();
            preventModalOpen();
        });
    } else {
        initializeSwaggerAuth();
        preventModalOpen();
    }

    window.swaggerAuthUtils = {
        debug: function () {
            return {
                currentToken: currentToken ? currentToken.substring(0, 20) + '...' : 'None',
                swaggerUI: window.ui,
                authState: window.ui?.getState?.()?.auth
            };
        },

        setToken: function (token) {
            currentToken = token;
            setAuthorizationSilently(token);
        },

        clearToken: function () {
            currentToken = null;
            if (window.ui && window.ui.authActions) {
                window.ui.authActions.logout();
            }
        }
    };

})();
