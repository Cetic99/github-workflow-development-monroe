/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  position: relative;
  width: 6.25rem;
  height: 6.25rem;
  margin: 0.5rem;

  .outer {
    position: absolute;
    border-radius: 50%;
    border-style: solid;
    border-color: ${(p) => `${p.color} ${p.color} ${p.color} transparent`};
    animation: spin 2s linear infinite;

    top: 50%;
    left: 50%;
    width: 6.25rem;
    height: 6.25rem;
    border-width: 0.625rem;
    animation-direction: normal;
  }

  .inner {
    position: absolute;
    border-radius: 50%;
    border-style: solid;
    border-color: ${(p) => `${p.color} ${p.color} transparent transparent`};
    animation: spin 2s linear infinite;

    width: 3.75rem;
    height: 3.75rem;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    border-width: 0.625rem;
    animation-direction: reverse;
  }
`

const Spinner = ({ color = 'var(--secondary-dark)', size = 'm' }) => {
  return (
    <Container color={color} size={size}>
      <div className="arc outer"></div>
      <div className="arc inner"></div>
    </Container>
  )
}

export default Spinner
