/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconBackspace from '@ui/components/icons/IconBackspace'

const Container = styled.div`
  width: fit-content;
  display: flex;
  flex-direction: column;
  gap: 0.5rem;

  & .row {
    display: flex;
    gap: 0.5rem;
  }

  & .item {
    transition: all 0.25s ease;
    display: flex;
    align-items: center;
    justify-content: center;

    height: 4.5rem;
    width: 9rem;
    border-radius: 12px;
    font-family: Poppins;
    font-weight: 600;
    font-style: SemiBold;
    font-size: 2.75rem;
    line-height: 3rem;
    text-align: center;
    color: black;
  }

  & .item:active {
    opacity: 0.75;
  }

  & .number {
    background-color: #ccd3c7;
  }

  & .action {
    background-color: #97aa8b;
  }
`

const Numpad = ({ onNumber = () => {}, onDelete = () => {}, onClear = () => {} }) => {
  return (
    <Container>
      <div className="row">
        <div className="item number" onClick={() => onNumber(1)}>
          1
        </div>
        <div className="item number" onClick={() => onNumber(2)}>
          2
        </div>
        <div className="item number" onClick={() => onNumber(3)}>
          3
        </div>
      </div>

      <div className="row">
        <div className="item number" onClick={() => onNumber(4)}>
          4
        </div>
        <div className="item number" onClick={() => onNumber(5)}>
          5
        </div>
        <div className="item number" onClick={() => onNumber(6)}>
          6
        </div>
      </div>

      <div className="row">
        <div className="item number" onClick={() => onNumber(7)}>
          7
        </div>
        <div className="item number" onClick={() => onNumber(8)}>
          8
        </div>
        <div className="item number" onClick={() => onNumber(9)}>
          9
        </div>
      </div>

      <div className="row">
        <div className="item action" onClick={onDelete}>
          <IconBackspace size="l" />
        </div>
        <div className="item number" onClick={() => onNumber(0)}>
          0
        </div>
        <div className="item action" onClick={onClear}>
          C
        </div>
      </div>
    </Container>
  )
}

export default Numpad
