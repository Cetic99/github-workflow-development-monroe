/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  gap: 1.25rem;
  user-select: none;
  transition: all 0.25s ease;
  position: relative;
  padding: 2rem 2.5rem 2rem 2rem;
  background: white;
  border-radius: 16px;
  box-shadow: 0px 40px 40px 0px #0000000d;

  width: ${(p) => (p.fullWidth ? '100%' : 'fit-content')};
  min-width: 20rem;

  &:active {
    opacity: 0.75;
  }

  & .text {
    display: flex;
    align-items: center;

    font-weight: 700;
    font-style: Bold;
    font-size: 2.5rem;
    line-height: 3rem;
    color: black;
  }
`

const ImageCardV1 = ({ fullWidth = false, image = () => <></>, text, onClick }) => {
  //
  const handleClick = (event) => {
    onClick(event)
  }

  return (
    <Container onClick={handleClick} fullWidth={fullWidth}>
      <div className="image">{image()}</div>
      <div className="text">{text}</div>
    </Container>
  )
}

export default ImageCardV1
