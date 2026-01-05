/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  .loader-track {
    position: relative;
    width: 100%;
    height: 6px;
    background: var(--bg-medium);
    border-radius: 9999px;
    overflow: hidden;
  }

  .loader-bar {
    position: absolute;
    left: -40%;
    top: 0;
    height: 100%;
    width: 40%;
    background: var(--primary-dark);
    border-radius: 9999px;
    animation: slide 1.2s ease-in-out infinite;
  }

  @keyframes slide {
    0% {
      left: -40%;
    }
    50% {
      left: 60%;
    }
    100% {
      left: 100%;
    }
  }
`

const BarLoader = ({ loading = false }) => {
  return (
    <Container>
      <div className="loader-track">{loading && <div className="loader-bar"></div>}</div>
    </Container>
  )
}

export default BarLoader
