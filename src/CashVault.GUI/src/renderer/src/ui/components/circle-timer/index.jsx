/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { CountdownCircleTimer } from 'react-countdown-circle-timer'

const Container = styled.div`
  .remaining-time {
    font-weight: 400;
    font-size: 1.625rem;
    line-height: 2.375rem;
  }
`

const CircularTimer = (props) => {
  const { className = '', duration = 10, size = 96, strokeWidth = 4, ...rest } = props

  return (
    <Container className={className}>
      <CountdownCircleTimer
        isPlaying
        duration={duration}
        colors={['var(--secondary-dark)']}
        size={size}
        strokeWidth={strokeWidth}
        {...rest}
      >
        {({ remainingTime }) => <div className="remaining-time">{remainingTime}</div>}
      </CountdownCircleTimer>
    </Container>
  )
}

export default CircularTimer
