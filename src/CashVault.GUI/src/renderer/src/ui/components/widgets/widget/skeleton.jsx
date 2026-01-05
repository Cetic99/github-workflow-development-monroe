/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  width: 100%;
  height: 11.5rem;
  border-radius: 1rem;

  background: linear-gradient(
    90deg,
    var(--bg-gray-medium) 25%,
    var(--bg-medium) 50%,
    var(--bg-dark) 75%
  );
  background-size: 200% 100%;

  animation: shimmer 1.5s infinite linear;

  @keyframes shimmer {
    0% {
      background-position: -200% 0;
    }
    100% {
      background-position: 200% 0;
    }
  }
`

const WidgetSkeleton = () => {
  return <Container />
}

export default WidgetSkeleton
