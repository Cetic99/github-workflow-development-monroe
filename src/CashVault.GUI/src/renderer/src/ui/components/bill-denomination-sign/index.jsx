/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;

  & .denom-svg {
    display: block;
    width: 7.25rem;
    height: 4.8125rem;

    ${(p) => {
      if (p.size === 'l') {
        return `
           width: 7.25rem;
           height: 4.8125rem;
        `
      }

      if (p.size === 'm') {
        return `
           width: 5.25rem;
           height: 3.5rem;
        `
      }
    }}
  }

  & span {
    color: var(--secondary-dark);
    font-weight: 600;
    font-size: 2.125rem;
    line-height: 2.375rem;
    letter-spacing: -3%;

    position: absolute;
    top: 53.5%;
    left: 50%;
    transform: translate(-50%, -50%);
  }
`

const BillDenominationSing = ({ value = 0, size = 'l' }) => {
  return (
    <Container size={size}>
      <svg
        className="denom-svg"
        width="116"
        height="74"
        viewBox="0 0 116 74"
        fill="none"
        xmlns="http://www.w3.org/2000/svg"
      >
        <path
          d="M13.6364 0C6.10455 0 0 6.04395 0 13.5V60.5C0 67.9565 6.10455 74 13.6364 74H97.8182H102.364C109.895 74 116 67.9565 116 60.5V13.5C116 6.04395 109.895 0 102.364 0H13.6364ZM21.95 9.04951L94.1364 8.96941C95.6091 14.0535 101.145 18 106.909 18V56C101.145 56 95.9591 59.681 94.0545 65.009L21.8864 64.9775C20.0864 59.762 14.8545 56 9.09091 56V18C14.8545 18 20.05 14.265 21.95 9.04951Z"
          fill="#0D8472"
        />
      </svg>

      <span>{value}</span>
    </Container>
  )
}

export default BillDenominationSing
