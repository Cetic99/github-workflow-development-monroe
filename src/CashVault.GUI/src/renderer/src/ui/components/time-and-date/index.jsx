/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useState, useEffect } from 'react'
import { useLocalTime } from '@domain/administration/stores/regional'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 0.125rem;
  color: var(--primary-light);

  & .time {
    font-weight: 700;
    font-size: 2rem;
    line-height: 1.875rem;
    letter-spacing: 0%;
  }

  & .date {
    font-weight: 600;
    font-size: 0.688rem;
    line-height: 1.125rem;
    letter-spacing: 4%;
    text-transform: uppercase;
  }
`

const TimeAndDate = () => {
  const [time, setTime] = useState(new Date())

  const { date, hours, minutes } = useLocalTime(time)

  useEffect(() => {
    const timer = setInterval(() => {
      setTime(new Date())
    }, 60000)

    return () => clearInterval(timer)
  }, [])


  return (
    <Container>
      <div className="time">{`${hours}:${minutes}`}</div>
      <div className="date">{date}</div>
    </Container>
  )
}

export default TimeAndDate
