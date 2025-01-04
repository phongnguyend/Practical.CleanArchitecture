import React from "react";
import { createRoot } from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";
import { configureStore, combineReducers } from "@reduxjs/toolkit";
import createSagaMiddleware from "redux-saga";

import "./index.css";
import App from "./App";
import { loadUser, isAuthenticated } from "./containers/Auth/authService";
import authReducer from "./containers/Auth/reducer";
import configurationEntryReducer from "./containers/Settings/reducer";
import { watchConfigurationEntry } from "./containers/Settings/sagas";
import productReducer from "./containers/Products/reducer";
import { watchProduct } from "./containers/Products/sagas";
import userReducer from "./containers/Users/reducer";
import { watchUser } from "./containers/Users/sagas";

const rootReducer = combineReducers({
  auth: authReducer,
  configurationEntry: configurationEntryReducer,
  product: productReducer,
  user: userReducer,
});

const sagaMiddleware = createSagaMiddleware();

const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({ thunk: false }).concat(sagaMiddleware),
  devTools: process.env.NODE_ENV == "development",
});

sagaMiddleware.run(watchConfigurationEntry);
sagaMiddleware.run(watchProduct);
sagaMiddleware.run(watchUser);

loadUser()
    .then((user) => {
        if (isAuthenticated()) {
            store.dispatch({
                type: "LOGIN",
                user: { access_token: user!.access_token, expires_in: user!.expires_in },
            });
        }
    })
    .finally(() => {
        const container = document.getElementById("root");
        const root = createRoot(container!);
        root.render(
            <React.StrictMode>
                <Provider store={store}>
                    <BrowserRouter>
                        <App />
                    </BrowserRouter>
                </Provider>
            </React.StrictMode>
        );
    });
