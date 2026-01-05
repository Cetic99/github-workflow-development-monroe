import TakeCard from '@ui/assets/images/atm/take-credit-card.png'
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  height: fit-content;

  & .credit-card {
    width: 181px;
    height: 250px;
    animation: slide-out 1s linear forwards;
  }

  @keyframes slide-out {
    0% {
      transform: translateY(-20px);
    }
    100% {
      transform: translateY(0px);
    }
  }
`

const IconTakeCard = () => {
  return (
    <Container>
      <svg
        width="180"
        height="24"
        viewBox="0 0 180 24"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <rect
          x="1"
          y="1"
          width="178"
          height="22"
          rx="4"
          fill="#3B4150"
          stroke="#787F90"
          stroke-width="2"
        />
        <rect x="10" y="8" width="160" height="8" rx="2" fill="#08162A" />
      </svg>

      <img className="credit-card" src={TakeCard} />
    </Container>
  )
}

export default IconTakeCard
