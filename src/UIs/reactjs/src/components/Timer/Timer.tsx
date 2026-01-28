import { useState, useEffect } from "react";

function debounce(func, delay) {
  let debounceTimer;
  return function () {
    const context = this;
    const args = arguments;
    clearTimeout(debounceTimer);
    debounceTimer = setTimeout(() => func.apply(context, args), delay);
  };
}

const handleMouseMove = debounce(function (event) {
  console.log("Mouse moved!", event);
}, 100); // Adjust the delay as needed (100ms in this case)

const Timer = () => {
  const [years, setYears] = useState(0);
  const [months, setMonths] = useState(0);
  const [days, setDays] = useState(0);
  const [hours, setHours] = useState(0);
  const [minutes, setMinutes] = useState(0);
  const [seconds, setSeconds] = useState(0);

  const getTime = () => {
    var lastActiveTime = Date.parse(localStorage.getItem("lastActiveTime"));
    if (lastActiveTime && lastActiveTime < Date.now() - 1000 * 3) {
      alert("You have been inactive for more than 3 seconds. Please refresh the page.");
    }

    const currentDateTime = new Date();
    setYears(currentDateTime.getFullYear());
    setMonths(currentDateTime.getMonth() + 1);
    setDays(currentDateTime.getDate());
    setHours(currentDateTime.getHours());
    setMinutes(currentDateTime.getMinutes());
    setSeconds(currentDateTime.getSeconds());

    localStorage.setItem("lastActiveTime", currentDateTime.toString());
  };

  useEffect(() => {
    document.addEventListener("mousemove", handleMouseMove);

    document.addEventListener("keydown", () => {
      console.log("Key pressed");
    });

    document.addEventListener("mousedown", () => {
      console.log("Mouse button clicked");
    });

    document.addEventListener("scroll", () => {
      console.log("Page scrolled");
    });

    window.addEventListener("focus", () => {
      console.log("Window focused");
    });

    const interval = setInterval(() => getTime(), 1000);
    return () => clearInterval(interval);
  }, []);

  return (
    <>
      <span>
        {years}-{months < 10 ? "0" + months : months}-{days < 10 ? "0" + days : days}{" "}
        {hours < 10 ? "0" + hours : hours}:{minutes < 10 ? "0" + minutes : minutes}:
        {seconds < 10 ? "0" + seconds : seconds}
      </span>
    </>
  );
};

export default Timer;
