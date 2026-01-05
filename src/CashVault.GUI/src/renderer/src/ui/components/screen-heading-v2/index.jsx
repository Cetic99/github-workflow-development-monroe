/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 1rem;

  & .top {
    padding-bottom: 0.5rem;
    font-weight: 600;
    font-style: SemiBold;
    font-size: 1.5rem;
    line-height: 2rem;
    text-transform: uppercase;
    color: var(--primary-light);
  }

  & .middle {
    font-weight: 700;
    font-style: Bold;
    font-size: 4.375rem;
    leading-trim: NONE;
    line-height: 4.75rem;
    color: white;
  }

  & .bottom {
    padding-top: 0.5rem;
    font-weight: 400;
    font-style: Regular;
    font-size: 1.625rem;
    line-height: 2rem;
    color: var(--primary-light);
  }
`

const ScreenHeadingV2 = ({ top, middle, bottom }) => {
  return (
    <Container>
      {top && <div className="top">{top()}</div>}
      {middle && <div className="middle">{middle}</div>}
      {bottom && <div className="bottom">{bottom()}</div>}
    </Container>
  )
}

export default ScreenHeadingV2
