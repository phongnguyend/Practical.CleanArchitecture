import { useState, useEffect } from "react";

const Timer = () => {
  const [years, setYears] = useState(0);
  const [months, setMonths] = useState(0);
  const [days, setDays] = useState(0);
  const [hours, setHours] = useState(0);
  const [minutes, setMinutes] = useState(0);
  const [seconds, setSeconds] = useState(0);

  const getTime = () => {
    const currentDateTime = new Date();
    setYears(currentDateTime.getFullYear());
    setMonths(currentDateTime.getMonth() + 1);
    setDays(currentDateTime.getDate());
    setHours(currentDateTime.getHours());
    setMinutes(currentDateTime.getMinutes());
    setSeconds(currentDateTime.getSeconds());
  };

  useEffect(() => {
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
