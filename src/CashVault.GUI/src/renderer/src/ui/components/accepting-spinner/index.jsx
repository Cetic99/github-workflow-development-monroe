/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconDollarCoin from '@ui/components/icons/IconDollarCoin'

const Container = styled.div`
  position: relative;
  width: 8.25rem;
  height: 8.25rem;
  margin: 0.5rem;

  .outer {
    position: absolute;
    border-radius: 50%;
    border-style: solid;
    border-color: ${(p) => `${p.color} ${p.color} ${p.color} transparent`};
    animation: spin 2s linear infinite;

    top: 50%;
    left: 50%;
    width: 8.25rem;
    height: 8.25rem;
    border-width: 0.625rem;
    animation-direction: normal;
  }

  .inner {
    position: absolute;
    border-radius: 50%;
    border-style: solid;
    border-color: ${(p) => `${p.color} ${p.color} transparent transparent`};
    animation: spin 2s linear infinite;

    width: 5.75rem;
    height: 5.75rem;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    border-width: 0.625rem;
    animation-direction: reverse;
  }

  & .inner-icon {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -47%);
  }
`

const AcceptingSpinner = ({ color = 'var(--secondary-dark)', size = 'm' }) => {
  return (
    <Container color={color} size={size}>
      <div className="arc outer"></div>
      <div className="arc inner"></div>

      <div className="inner-icon">
        <IconDollarCoin size="l" color="var(--primary-dark)" />
      </div>
    </Container>
  )
}

export default AcceptingSpinner
