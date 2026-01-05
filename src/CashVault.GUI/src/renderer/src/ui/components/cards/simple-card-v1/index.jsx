/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
  user-select: none;
  transition: all 0.25s ease;
  position: relative;
  padding: 2rem;
  background: white;
  border-radius: 16px;
  box-shadow: 0px 40px 40px 0px #0000000d;

  width: ${(p) => (p.fullWidth ? '100%' : 'fit-content')};
  min-width: 20rem;

  &:active {
    opacity: 0.75;
  }

  & .text {
    font-weight: 600;
    font-style: SemiBold;
    font-size: 2rem;
    line-height: 2.5rem;
    color: black;
  }

  & .tag {
    position: absolute;
    top: 0;
    right: 0;
    padding: 0.25rem 0.5rem;

    background-color: var(--secondary-dark);
    font-weight: 600;
    font-style: SemiBold;
    font-size: 1.5rem;
    line-height: 2rem;
    color: white;

    border-radius: 0 1rem 0 1rem;
  }
`

const SimpleCardV1 = ({ fullWidth = false, icon, text, tag, onClick }) => {
  //
  const handleClick = (event) => {
    onClick(event)
  }

  return (
    <Container onClick={handleClick} fullWidth={fullWidth}>
      {tag && <div className="tag">{tag}</div>}

      <div className="icon">{icon({ size: 'l', color: 'black' })}</div>
      <div className="text">{text}</div>
    </Container>
  )
}

export default SimpleCardV1
