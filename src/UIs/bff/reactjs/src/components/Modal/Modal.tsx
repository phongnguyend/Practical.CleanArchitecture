"use client";

import {
  Children,
  isValidElement,
  createContext,
  useContext,
  useEffect,
  useRef,
  useState,
  type HTMLAttributes,
  type ReactNode,
} from "react";
import { createPortal } from "react-dom";
import { mountModal } from "./modalRuntime";
import "./modal.css";

const CloseContext = createContext<() => void>(() => {});
interface ModalProps {
  show: boolean;
  onHide: () => void;
  size?: "sm" | "lg" | "xl";
  children: ReactNode;
}

function Modal({ show, onHide, size, children }: ModalProps) {
  const [mounted, setMounted] = useState(false);
  useEffect(() => setMounted(true), []);
  const layer = useRef<HTMLDivElement>(null);
  const panel = useRef<HTMLElement>(null);
  const close = useRef(onHide);
  close.current = onHide;
  useEffect(() => {
    if (mounted && show && layer.current && panel.current) {
      return mountModal(layer.current, panel.current, () => close.current());
    }
  }, [show, mounted]);
  if (!mounted || !show) return null;
  return createPortal(
    <CloseContext.Provider value={onHide}>
      <div className="app-modal-layer" ref={layer}>
        <section
          className={`app-modal-panel${size ? ` app-modal-panel--${size}` : ""}`}
          ref={panel}
          role="dialog"
          aria-modal="true"
          tabIndex={-1}
        >
          {children}
          {!hasFooter(children) && (
            <Footer>
              <button type="button" className="btn btn-secondary" onClick={onHide}>
                Close
              </button>
            </Footer>
          )}
        </section>
      </div>
    </CloseContext.Provider>,
    document.body
  );
}

function Header({
  children,
  closeButton = true,
  className = "",
  ...props
}: HTMLAttributes<HTMLElement> & { closeButton?: boolean }) {
  const close = useContext(CloseContext);
  return (
    <header className={`app-modal-header ${className}`} {...props}>
      {children}
      {closeButton && (
        <button type="button" className="app-modal-close" onClick={close} aria-label="Close dialog">
          &#215;
        </button>
      )}
    </header>
  );
}
function Title({ className = "", ...props }: HTMLAttributes<HTMLHeadingElement>) {
  return <h2 className={`app-modal-title ${className}`} data-modal-title="" {...props} />;
}
function Body({ className = "", ...props }: HTMLAttributes<HTMLDivElement>) {
  return <div className={`app-modal-body ${className}`} {...props} />;
}
function Footer({ className = "", ...props }: HTMLAttributes<HTMLElement>) {
  return <footer className={`app-modal-footer ${className}`} {...props} />;
}

function hasFooter(children: ReactNode): boolean {
  return Children.toArray(children).some(
    (child) =>
      isValidElement<{ children?: ReactNode }>(child) &&
      (child.type === Footer || hasFooter(child.props.children))
  );
}

export default Object.assign(Modal, { Header, Title, Body, Footer });
