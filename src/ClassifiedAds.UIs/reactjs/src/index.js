import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import { Provider } from "react-redux";
import { createStore, applyMiddleware, compose, combineReducers } from "redux";
import createSagaMiddleware from "redux-saga";
import axios from "axios";

import "./index.css";
import App from "./App";
import * as serviceWorker from "./serviceWorker";
import productReducer from "./containers/Products/reducer";
import { watchProduct } from "./containers/Products/sagas";
import AuthService from "./containers/Auth/authService";
import authReducer from "./containers/Auth/reducer";
import auditLogReducer from "./containers/AuditLogs/reducer";
import { watchAuditLog } from "./containers/AuditLogs/sagas";
import fileReducer from "./containers/Files/reducer";
import { watchFile } from "./containers/Files/sagas";

const composeEnhancers =
  (process.env.NODE_ENV === "development"
    ? window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__
    : null) || compose;

const rootReducer = combineReducers({
  auth: authReducer,
  file: fileReducer,
  product: productReducer,
  auditLog: auditLogReducer,
});

const sagaMiddleware = createSagaMiddleware();

const store = createStore(
  rootReducer,
  composeEnhancers(applyMiddleware(sagaMiddleware))
);

sagaMiddleware.run(watchFile);
sagaMiddleware.run(watchProduct);
sagaMiddleware.run(watchAuditLog);

store.dispatch({
  type: "SET_AUTH_SERVICE",
  authService: AuthService,
});

AuthService.loadUser().then((user) => {
  if (AuthService.isAuthenticated()) {
    store.dispatch({
      type: "LOGIN",
      user: user,
    });
  }

  ReactDOM.render(
    <React.StrictMode>
      <Provider store={store}>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </Provider>
    </React.StrictMode>,
    document.getElementById("root")
  );

  // If you want your app to work offline and load faster, you can change
  // unregister() to register() below. Note this comes with some pitfalls.
  // Learn more about service workers: https://bit.ly/CRA-PWA
  serviceWorker.unregister();
});
